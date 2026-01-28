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
		var games = await dbContext.BotGames.Where(x => x.IsGlobal).ToListAsync();
		return Ok(games.Select(x => new
		{
			x.Id,
			x.GameUrl,
			Order = x.GlobalOrder,
		}).OrderByDescending(x => x.Order));
	}
	
	[HttpGet("{pcId}")]
	public async Task<IActionResult> GetOverridenBotGames(int pcId)
	{
		var pc = await dbContext.Pcs
			.Include(x => x.OverrideBotGames)
			.ThenInclude(x => x.BotGame)
			.FirstOrDefaultAsync(x => x.Id == pcId);
		
		if (pc == null)
			return NotFound();
		
		return Ok(pc.OverrideBotGames.Select(x => new
		{
			x.BotGame.Id,
			x.BotGame.GameUrl,
			x.Order
		}).OrderByDescending(x => x.Order));
	}
		
	
	[HttpPost]
	public async Task<IActionResult> AddGlobalBotGame([FromBody] BotGame? game)
	{
		if (game == null)
			return BadRequest();
		
		var maxOrder = await dbContext.BotGames
			.Where(x => x.IsGlobal)
			.MaxAsync(x => (int?)x.GlobalOrder) ?? 0;

		game.IsGlobal = true;
		game.GlobalOrder = maxOrder + 1;
		
		dbContext.BotGames.Add(game);
			
		await dbContext.SaveChangesAsync();
		await agentGamesUpdater.UpdateGamesDefault();
		return Ok(game);
	}
	
	[HttpPost("{pcId}")]
	public async Task<IActionResult> AddOverridenBotGame(int pcId, [FromBody] BotGame? game)
	{
		var pc = await dbContext.Pcs
			.Include(x => x.OverrideBotGames)
			.ThenInclude(x => x.BotGame)
			.FirstOrDefaultAsync(x => x.Id == pcId);

		if (pc == null)
			return NotFound();
		
		if (game == null)
			return BadRequest();

		game.IsGlobal = false;

		dbContext.BotGames.Add(game);
		
		await dbContext.SaveChangesAsync();
		
		var maxOrder = await dbContext.PcModelToBotGames
			.Where(x => x.PcModelId == pc.Id)
			.MaxAsync(x => (int?)x.Order) ?? 0;
		
		dbContext.PcModelToBotGames.Add(new PcModelToBotGame
		{
			PcModelId = pc.Id,
			BotGameId = game.Id,
			Order = maxOrder + 1
		});
			
		await dbContext.SaveChangesAsync();
		await agentGamesUpdater.UpdateGamesOverride(pc.BotId);
		return Ok(game);
	}
		
	
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteBotGame(int id)
	{
		var botGame = await dbContext.BotGames.FindAsync(id);
			
		if (botGame == null) 
			return NotFound();

		var pcModelToBotGame = await dbContext.PcModelToBotGames.Include(x => x.PcModel).FirstOrDefaultAsync(x => x.BotGameId == id);

		var pcModel = pcModelToBotGame?.PcModel;
		
		if (pcModelToBotGame != null) 
			dbContext.PcModelToBotGames.Remove(pcModelToBotGame);

		dbContext.BotGames.Remove(botGame);
		
		await dbContext.SaveChangesAsync();
		
		if(pcModel == null)
			await agentGamesUpdater.UpdateGamesDefault();
		else
			await agentGamesUpdater.UpdateGamesOverride(pcModel.BotId);
		
		return NoContent();
	}
	
	
	[HttpPost("reorder")]
	public async Task<IActionResult> ReorderGlobalGames([FromBody] int[] orderedGameIds)
	{
		var games = await dbContext.BotGames
			.Where(x => x.IsGlobal)
			.ToListAsync();

		var map = games.ToDictionary(x => x.Id);

		for (int i = 0; i < orderedGameIds.Length; i++)
		{
			map[orderedGameIds[i]].GlobalOrder = i + 1;
		}

		await dbContext.SaveChangesAsync();
		await agentGamesUpdater.UpdateGamesDefault();

		return Ok();
	}
	
	[HttpPost("reorder/{pcModelId}")]
	public async Task<IActionResult> ReorderOverridenBotGames(int pcModelId, [FromBody] int[] orderedGameIds)
	{
		var links = await dbContext.PcModelToBotGames
			.Where(x => x.PcModelId == pcModelId)
			.ToListAsync();

		var pc = await dbContext.Pcs.FindAsync(pcModelId);
		
		var map = links.ToDictionary(x => x.BotGameId);
		
		for (int i = 0; i < orderedGameIds.Length; i++)
		{
			map[orderedGameIds[i]].Order = i + 1;
		}

		await dbContext.SaveChangesAsync();
		
		if(pc != null)
			await agentGamesUpdater.UpdateGamesOverride(pc.BotId);
		
		return Ok();
	}
}