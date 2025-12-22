using AnydeskTracker.Data;
using AnydeskTracker.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnydeskTracker.Controllers;


[Authorize(Roles = "Admin")]
[Route("Admin")]
public class AdminController(ApplicationDbContext context) : Controller
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
	
	[HttpGet("Games")]
	public async Task<IActionResult> Games()
	{
		return View("Games");
	}

	[HttpGet("Users")]
	public async Task<IActionResult> Users()
	{
		return View("Users");
	}

	[HttpGet("User/{userId}")]
	public async Task<IActionResult> GetUser(string userId)
	{
		var user = await context.Users.FindAsync(userId);
		
		if (user == null)
			return NotFound();
		
		var sessions = context.WorkSessionModels.Where(x => x.User.Id == userId).ToArray();

		return View("User", new AdminUserPageDto(user, sessions));
	}

	[HttpGet("Bots")]
	public async Task<IActionResult> Bots()
	{
		return View("Bots");
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
		
		return View("Bot", new AdminBotPageDto(
			pc,
			lastDolphinCheck,
			botLogDates.Order().Select(x => new AdminBotPageDto.BotLog(x, dolphinLogs.Count(y => y.Timestamp.Date == x))).ToArray()));
	}
}