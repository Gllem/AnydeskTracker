using AnydeskTracker.Data;
using AnydeskTracker.DTOs;
using AnydeskTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnydeskTracker.Controllers;


[Authorize(Roles = "Admin")]
[Route("Admin")]
public class AdminController(ApplicationDbContext context, PcService pcService) : Controller
{
	[HttpGet]
	public async Task<IActionResult> Index()
	{
		return View("AdminIndex");
	}
	
	[HttpGet("Computers")]
	public async Task<IActionResult> Computers()
	{
		return View("Computers");
	}
	
	[HttpGet("GameCatalog")]
	public async Task<IActionResult> GameCatalog()
	{
		return View("Games/GameCatalog");
	}
	
	[HttpGet("GameSchedules")]
	public async Task<IActionResult> GameSchedules()
	{
		return View("Games/GameSchedules");
	}

	[HttpGet("Users")]
	public async Task<IActionResult> Users()
	{
		return View("Users/Users");
	}

	[HttpGet("User/{userId}")]
	public async Task<IActionResult> GetUser(string userId)
	{
		var user = await context.Users.FindAsync(userId);
		
		if (user == null)
			return NotFound();
		
		var sessions = context.WorkSessionModels.Where(x => x.User.Id == userId).ToArray();

		return View("Users/User", new AdminUserPageDto(user, sessions));
	}

	[HttpGet("Bots")]
	public async Task<IActionResult> Bots()
	{
		return View("Bots/Bots");
	}
	
	[HttpGet("Bot/{pcModelId}")]
	public async Task<IActionResult> GetBot(int pcModelId)
	{
		var pc = await context.Pcs.FindAsync(pcModelId);
		
		if (pc == null)
			return NotFound();

		var botLogs = await context.BotActions.Where(x => x.PcId == pc.Id).ToListAsync();
		var dolphinLogs = await context.DolphinActions.Where(x => x.PcId == pc.Id).ToListAsync();
		var botLogDates = 
			botLogs.Select(x => x.Timestamp.Date)
			.Concat(dolphinLogs.Select(x => x.Timestamp.Date)).Distinct().ToArray();

		DateTime? lastDolphinCheck = dolphinLogs.Count > 0 ? dolphinLogs.Max(x => x.Timestamp) : null;
		
		return View("Bots/BotStatus", new AdminBotPageDto(
			pc,
			lastDolphinCheck,
			botLogDates.Order().Select(x => new AdminBotPageDto.BotLog(x, dolphinLogs.Count(y => y.Timestamp.Date == x))).ToArray()));
	}
	
	[HttpGet("BotGames")]
	public async Task<IActionResult> BotGames()
	{
		var pcs = await pcService.GetAllPcs();
		var games = await context.GameCatalog.Select(x => new GameDto(x)).ToListAsync();
		
		return View("Bots/BotGames", new AdminBotGamesDto(pcs.ToArray(), games.ToArray()));
	}
}