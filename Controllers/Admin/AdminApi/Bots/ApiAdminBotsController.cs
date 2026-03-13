using AnydeskTracker.Data;
using AnydeskTracker.DTOs;
using AnydeskTracker.Extensions;
using AnydeskTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnydeskTracker.Controllers.Bots;

[Authorize(Roles = "Admin")]
[Route("api/admin/bots")]
[ApiController]
public class ApiAdminBotsController(ApplicationDbContext dbContext) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetAllBots()
	{
		var pcs = await dbContext.Pcs.ToListAsync();
		var botActions = await dbContext.BotActions.OrderByDescending(x => x.Timestamp).ToListAsync();
		var dolphinChecks = await dbContext.DolphinActions.OrderByDescending(x => x.Timestamp).ToListAsync();
		var todayDolphinChecks = dolphinChecks.Where(x => x.Timestamp.Date == DateTime.UtcNow.Date);
		return Ok(pcs.Select(pc =>
		{
			var lastAction = botActions.FirstOrDefault(x => x.PcId == pc.Id);

			var lastDolphinAction = dolphinChecks.FirstOrDefault(x => x.PcId == pc.Id);
			var dolphinChecksCount = todayDolphinChecks.Count(x => x.PcId == pc.Id);

			List<string> errorStatuses = new List<string> { };
				
			if (lastAction != null && (lastDolphinAction == null || lastDolphinAction.Timestamp < lastAction.Timestamp))
			{
				if(!string.IsNullOrEmpty(lastAction.ProcessesStatus))
					errorStatuses.Add("PRC");
				if(!string.IsNullOrEmpty(lastAction.DiskStatus))
					errorStatuses.Add("DSK");
				if(!string.IsNullOrEmpty(lastAction.RamStatus))
					errorStatuses.Add("RAM");
				if(!string.IsNullOrEmpty(lastAction.UserStatus))
					errorStatuses.Add("USR");
			}
			
			if(pc.AhkError)
				errorStatuses.Add("AHK");

			string status = errorStatuses.Count > 0 ? "ERROR" : "OK";
				
			if (pc.Status == PcStatus.Busy)
				status = "BUSY";
			if (pc.Status == PcStatus.CoolingDown)
				status = "COOLING";
			
			var agentState = AgentPresence.Get(pc.BotId);

			string agentStatus = 
				agentState == null ? "not-found" : 
				agentState.Online ?
					"online" :
					"offline";

			return new
			{
				pc.BotId,
				pc.AnyDeskId,
				PcModelId = pc.Id,
				HasChecks = lastAction != null || errorStatuses.Count > 0,
				ErrorStatuses = errorStatuses,
				Status = status,
				AgentStatus = agentStatus,
				LastCheckTime = lastAction?.Timestamp.ToUtc(),
				LastDolphinCheckTime = lastDolphinAction?.Timestamp.ToUtc(),
				dolphinChecksCount
			};
		}));
	}
		
	[HttpPut("{pcModelId}")]
	public async Task<IActionResult> UpdateBotInfo(int pcModelId, [FromBody]AdminBotUpdateDto updateDto)
	{
		var pc = await dbContext.Pcs.FindAsync(pcModelId);
			
		if (pc == null)
			return NotFound("PC ID Not found");

		pc.BotId = updateDto.BotId;
		await dbContext.SaveChangesAsync();

		return Ok();
	}
		
	[HttpGet("{pcModelId}/actions/{date}")]
	public async Task<IActionResult> GetBotActions(int pcModelId, string date)
	{
		DateTime logsDateTime;
		try
		{
			int[] dateParse = date.Split(".").Select(int.Parse).ToArray();
			logsDateTime = new DateTime(dateParse[2], dateParse[1], dateParse[0]);
		}
		catch (Exception e)
		{
			return BadRequest($"Can't parse date: {e}");
		}
			
		var watchDogActions = await dbContext.BotActions
			.Where(x => x.PcId == pcModelId && x.Timestamp.Date == logsDateTime)
			.OrderByDescending(x => x.Timestamp)
			.ToListAsync();

		var dolphinActions = await dbContext.DolphinActions
			.Where(x => x.PcId == pcModelId && x.Timestamp.Date == logsDateTime)
			.OrderByDescending(x => x.Timestamp)
			.ToListAsync();
			
		return Ok(new
		{
			WatchDogActions = watchDogActions.Select(x => new
			{
				x.Error,
				Statuses = new Dictionary<string, string>
				{
					{"Выключенные процессы", x.ProcessesStatus},
					{"Диск", x.DiskStatus},
					{"Пользователь", x.UserStatus},
					{"Память", x.RamStatus}
				},
				timestamp = x.Timestamp.ToUtc()
			}),
			DolphinActions = dolphinActions.Select(x => new
			{
				Timestamp = x.Timestamp.ToUtc()
			})
		});
	}
}