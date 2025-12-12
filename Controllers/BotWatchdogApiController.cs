using AnydeskTracker.Data;
using AnydeskTracker.DTOs;
using AnydeskTracker.Models;
using AnydeskTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnydeskTracker.Controllers;

[Route("api/watchdog")]
[ApiController]
public class BotWatchdogApiController(ApplicationDbContext dbContext, TelegramService telegramService) : ControllerBase
{
	[HttpPost]
	public async Task<IActionResult> WatchdogWebhook([FromBody] BotWatchdogStatusDto statusDto)
	{
		var pc = await dbContext.Pcs.FirstOrDefaultAsync(x => x != null && x.BotId == statusDto.BotId);

		if (pc == null)
			return NotFound();
		
		Dictionary<string, string> statuses = statusDto.StatusChecks.ToDictionary(
					x => x.Key,
					x => x.Value ? "" : statusDto.ErrorDescriptions[x.Key]);
		
		var botAction = new PcBotAction
		{
			PcId = pc.Id,
			Error = statusDto.Error,
			ProcessesStatus = statuses["Processes"],
			SchedulerStatus = statuses["Scheduler"],
			DiskStatus = statuses["Disk"],
			UserStatus = statuses["User"],
			RamStatus = statuses["RAM"],
			Timestamp = DateTime.UtcNow
		};

		dbContext.BotActions.Add(botAction);

		await dbContext.SaveChangesAsync();

		if (botAction.Error)
		{
			await telegramService.SendMessageToAdmin(
				$"\u26a0\ufe0f Ошибка бота\n" +
				$"Бот: {pc.BotId}\n" +
				$"AnyDesk ID: {pc.PcId}\n\n" +
				$"Статус: \n" +
				botAction.TelegramNotificationBotStatus
			);
		}

		return Ok();
	}

	[HttpGet("dolphinStatus/{botId}")]
	public async Task<IActionResult> DolphinBotIdCheck(string botId)
	{
		var pc = await dbContext.Pcs.FirstOrDefaultAsync(x => x.BotId == botId);

		if (pc == null)
			return NotFound();

		dbContext.DolphinActions.Add(new PcBotDolphinAction
		{
			PcId = pc.Id,
			Timestamp = DateTime.UtcNow
		});
		
		await dbContext.SaveChangesAsync();

		return Ok(new
		{
			pc.PcId,
			pc.BotId
		});
	}
}