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

	[HttpGet("botGames/{botId}")]
	public async Task<IActionResult> GetBotGames(string botId)
	{
		var pc = await dbContext.Pcs.Include(x => x.OverridenBotGames).FirstOrDefaultAsync(x => x.BotId == botId);

		if (pc == null)
			return NotFound();

		return BadRequest();
		// return await GetGames(pc);
	}
	
	[HttpGet("botGamesId/{pcId}")]
	public async Task<IActionResult> GetBotGames(int pcId)
	{
		var pc = await dbContext.Pcs.Include(x => x.OverridenBotGames).FirstOrDefaultAsync(x => x.Id == pcId);

		if (pc == null)
			return NotFound();

		return BadRequest();
		// return await GetGames(pc);
	}
	
	[HttpGet("botGamesId/")]
	public async Task<IActionResult> GetBotGames()
	{
		return BadRequest();
		// return await GetGames(null);
	}

	// private async Task<IActionResult> GetGames(PcModel? pc)
	// {
	// 	IQueryable<BotGame> query;
	// 	
	// 	if (pc != null && pc.OverridenBotGames.Count != 0)
	// 		query = dbContext.PcModelToBotGames
	// 			.Where(x => x.PcModelId == pc.Id)
	// 			.OrderBy(x => x.Order)
	// 			.Select(x => x.BotGame);
	// 	else
	// 		query = dbContext.BotGames.Where(x => x.IsGlobal).OrderBy(x => x.GlobalOrder);
	//
	// 	var games = await query.ToListAsync();
	// 	
	// 	return File(
	// 		GetBotGamesFile(games),
	// 		"text/plain",
	// 		$"{pc?.DisplayId ?? "Games"}.txt"
	// 	);
	// }

	private byte[] GetBotGamesFile(List<Game> botGames)
	{
		var sb = new StringBuilder();
		foreach (var game in botGames)
		{
			sb.AppendLine(game.Url);
		}

		return Encoding.UTF8.GetBytes(sb.ToString());
	}
}