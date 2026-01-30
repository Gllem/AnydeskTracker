using AnydeskTracker.Data;
using AnydeskTracker.Models;
using AnydeskTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnydeskTracker.Controllers.Bots;

[Authorize(Roles = "Admin")]
[Route("api/admin/bots/games")]
[ApiController]
public class ApiAdminBotGamesController(ApplicationDbContext dbContext, AgentGamesUpdater agentGamesUpdater) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetGlobalBotGames()
	{
		var games = await dbContext.BotGameOrdersGlobal.Include(o => o.Game)
			.ToListAsync();
		return Ok(games.Select(x => new
		{
			x.Game.Id,
			x.Game.Url,
			x.Order,
		}).OrderByDescending(x => x.Order));
	}
	
	[HttpGet("{pcId}")]
	public async Task<IActionResult> GetOverridenBotGames(int pcId)
	{
		var pc = await dbContext.Pcs
			.Include(x => x.OverridenBotGames)
			.ThenInclude(x => x.Game)
			.FirstOrDefaultAsync(x => x.Id == pcId);
		
		if (pc == null)
			return NotFound();
		
		return Ok(pc.OverridenBotGames.Select(x => new
		{
			x.Game.Id,
			x.Game.Url,
			x.Order
		}).OrderByDescending(x => x.Order));
	}
}