using System.Text;
using AnydeskTracker.Data;
using AnydeskTracker.DTOs;
using AnydeskTracker.Models;
using AnydeskTracker.Models.Game;
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

		if (pc.Status is PcStatus.Busy or PcStatus.CoolingDown)
			return BadRequest("PC is busy");
		
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
				$"AnyDesk ID: {pc.AnyDeskId}\n\n" +
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
			pc.AnyDeskId,
			pc.BotId
		});
	}

#region GetGames
	[HttpGet("botGames/{botId}")]
	public async Task<IActionResult> GetBotGames(string botId)
	{
		var pc = await dbContext.Pcs.Include(x => x.OverridenBotGames).FirstOrDefaultAsync(x => x.BotId == botId);

		if (pc == null)
			return NotFound();

		return await GetGames(pc);
	}
	
	[HttpGet("botGamesId/{pcId}")]
	public async Task<IActionResult> GetBotGames(int pcId)
	{
		var pc = await dbContext.Pcs.Include(x => x.OverridenBotGames).FirstOrDefaultAsync(x => x.Id == pcId);

		if (pc == null)
			return NotFound();

		return await GetGames(pc);
	}
	
	[HttpGet("botGamesId/")]
	public async Task<IActionResult> GetBotGames()
	{
		return await GetGames(null);
	}

	private async Task<IActionResult> GetGames(PcModel? pc)
	{
		IQueryable<Game> query;
		
		if (pc != null && pc.OverridenBotGames.Count != 0)
			query = dbContext.BotGameAssignmentsOverride
				.Where(x => x.PcId == pc.Id)
				.OrderBy(x => x.Order)
				.Select(x => x.Game);
		else
			query = dbContext.BotGameAssignmentsGlobal.OrderBy(x => x.Order).Select(x => x.Game);
	
		var games = await query.ToListAsync();
		
		return File(
			GetBotGamesFile(games),
			"text/plain",
			$"{pc?.DisplayId ?? "Games"}.txt"
		);
	}

	private byte[] GetBotGamesFile(List<Game> botGames)
	{
		return Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, botGames.Select(x => x.Url)));
	}
#endregion

	[HttpGet("botSchedule/{botId}")]
	public async Task<IActionResult> GetBotSchedule(string botId)
	{
		var pc = await dbContext.Pcs.FirstOrDefaultAsync(x => x.BotId == botId);

		if (pc == null)
			return NotFound();

		return Ok(new
		{
			pc.PcBotSchedule.Enabled,
			pc.PcBotSchedule.StartTod,
			pc.PcBotSchedule.IntervalMinutes,
			pc.PcBotSchedule.EndTod
		});
	}

    [HttpGet("userGames/{botId}")]
    public async Task<IActionResult> GetUserGames(string botId)
    {
        var dayOfWeek = DateTime.UtcNow.ToLocalTime().DayOfWeek;
        var pcUsage = await dbContext.PcUsages.Include(pcUsage => pcUsage.WorkSession)
            .FirstOrDefaultAsync(x => x.IsActive && x.Pc.BotId == botId);
        if (pcUsage == null) return NotFound();

        var gameSchedules = await dbContext.GameSchedules.Include(gameSchedule => gameSchedule.UserLinks)
            .Include(gameSchedule => gameSchedule.Game)
            .Where(x => x.DayOfWeek == dayOfWeek).ToListAsync();

        return Ok(gameSchedules
            .Where(x => x.UserLinks.Any(y => y.UserId == pcUsage.WorkSession.UserId))
            .Select(x => new
            {
                GameName = x.Game.Name,
                GameId = x.Game.YandexMetrikaId
            }));
    }
	[HttpGet("botOccupation/{botId}")]
	public async Task<IActionResult> GetBotOccupation(string botId)
	{
		var pc = await dbContext.Pcs.FirstOrDefaultAsync(x => x.BotId == botId);

		if (pc == null)
			return NotFound();

		return Ok(pc.Status == PcStatus.Busy);
	}
}