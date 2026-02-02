using AnydeskTracker.Data;
using AnydeskTracker.DTOs;
using AnydeskTracker.Models;
using AnydeskTracker.Models.Game;
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
		var games = await dbContext.BotGameAssignmentsGlobal.Include(o => o.Game)
			.ToListAsync();
		return Ok(games.Select(x => new
		{
			x.Game.Id,
			x.Game.Url,
			x.Game.Name,
			x.Order,
		}).OrderByDescending(x => x.Order));
	}

	[HttpPost]
	public async Task<IActionResult> AddGlobalBotGame([FromBody] int gameId)
	{
		var existing = await dbContext.BotGameAssignmentsGlobal.FirstOrDefaultAsync(x => x.GameId == gameId);
		
		if (existing != null)
			return BadRequest();

		var game = await dbContext.GameCatalog.FindAsync(gameId);

		if (game == null)
			return BadRequest();
		
		var maxOrder = await dbContext.BotGameAssignmentsGlobal.MaxAsync(x => x.Order);

		var globalGameAssignment = new BotGameAssignmentGlobal
		{
			GameId = gameId,
			Order = maxOrder + 1
		};

		dbContext.BotGameAssignmentsGlobal.Add(globalGameAssignment);
		await dbContext.SaveChangesAsync();
		await agentGamesUpdater.UpdateGamesDefault();
		return Ok(new GameDto(game));
	}
	
	[HttpDelete]
	public async Task<IActionResult> RemoveGlobalBotGame([FromBody] int gameId)
	{
		var existing = await dbContext.BotGameAssignmentsGlobal.FirstOrDefaultAsync(x => x.GameId == gameId);
		
		if (existing == null)
			return NotFound();

		dbContext.BotGameAssignmentsGlobal.Remove(existing);
		
		await dbContext.SaveChangesAsync();
		await agentGamesUpdater.UpdateGamesDefault();
		return Ok();
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
			x.Game.Name,
			x.Order
		}).OrderByDescending(x => x.Order));
	}
	
	[HttpPost("{pcId}")]
	public async Task<IActionResult> AddOverridenBotGame(int pcId, [FromBody] int gameId)
	{
		var existing = 
			await dbContext.BotGameAssignmentsOverride.FirstOrDefaultAsync(x => x.GameId == gameId && x.PcId == pcId);
		
		if (existing != null)
			return BadRequest();

		var game = await dbContext.GameCatalog.FindAsync(gameId);

		if (game == null)
			return BadRequest();

		var pc = await dbContext.Pcs.FindAsync(pcId);

		if (pc == null)
			return BadRequest();
		
		var maxOrder = await dbContext.BotGameAssignmentsOverride.Where(x => x.PcId == pcId).MaxAsync(x => x.Order);

		var globalGameAssignment = new BotGameAssignmentOverride
		{
			GameId = gameId,
			PcId = pcId,
			Order = maxOrder + 1
		};

		dbContext.BotGameAssignmentsOverride.Add(globalGameAssignment);
		await dbContext.SaveChangesAsync();
		await agentGamesUpdater.UpdateGamesOverride(pc.BotId);
		return Ok(new GameDto(game));
	}
	
	[HttpDelete("{pcId}")]
	public async Task<IActionResult> RemoveOverridenBotGame(int pcId, [FromBody] int gameId)
	{
		var existing = await dbContext.BotGameAssignmentsOverride
			.Include(x => x.Pc)
			.FirstOrDefaultAsync(x => x.GameId == gameId && x.PcId == pcId);

		if (existing == null)
			return NotFound();
		
		var pc = existing.Pc;

		dbContext.BotGameAssignmentsOverride.Remove(existing);
		
		await dbContext.SaveChangesAsync();
		await agentGamesUpdater.UpdateGamesOverride(pc.BotId);
		return Ok();
	}

	[HttpPut("bulk-update")]
	public async Task<IActionResult> UpdateGamesOrderGlobal([FromBody] GameAssignmentDto[] assignmentDtos)
	{
		var globalAssignments = await dbContext.BotGameAssignmentsGlobal.ToListAsync();
		
		if (globalAssignments.Count != assignmentDtos.Length)
			return BadRequest();
		
		foreach (var assignmentDto in assignmentDtos)
		{
			if (globalAssignments.Count(x => x.GameId == assignmentDto.Id) != 1)
				return BadRequest();
		}

		var assignmentDict = assignmentDtos.ToDictionary(x => x.Id, x => x.SortOrder);

		foreach (var assignment in globalAssignments)
		{
			assignment.Order = assignmentDict[assignment.GameId];
		}

		await dbContext.SaveChangesAsync();
		await agentGamesUpdater.UpdateGamesDefault();
		return Ok();
	}

	[HttpPut("bulk-update/{pcId}")]
	public async Task<IActionResult> UpdateGamesOrderOverride(int pcId, [FromBody] GameAssignmentDto[] assignmentDtos)
	{
		var overridenAssignments = await dbContext.BotGameAssignmentsOverride.Where(x => x.PcId == pcId).ToListAsync();
		var pc = await dbContext.Pcs.FindAsync(pcId);

		if (pc == null)
			return NotFound();
		
		if (overridenAssignments.Count != assignmentDtos.Length)
			return BadRequest();
		
		foreach (var assignmentDto in assignmentDtos)
		{
			if (overridenAssignments.Count(x => x.GameId == assignmentDto.Id) != 1)
				return BadRequest();
		}

		var assignmentDict = assignmentDtos.ToDictionary(x => x.Id, x => x.SortOrder);

		foreach (var assignment in overridenAssignments)
		{
			assignment.Order = assignmentDict[assignment.GameId];
		}

		await dbContext.SaveChangesAsync();
		await agentGamesUpdater.UpdateGamesOverride(pc.BotId);
		return Ok();
	}
}