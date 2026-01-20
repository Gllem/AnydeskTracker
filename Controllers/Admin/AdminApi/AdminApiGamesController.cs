using AnydeskTracker.Data;
using AnydeskTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnydeskTracker.Controllers;


[Authorize(Roles = "Admin")]
[Route("api/admin/games")]
[ApiController]
public class AdminApiGamesController(ApplicationDbContext dbContext) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetAllGames()
	{
		var games = await dbContext.Games
			.Include(g => g.Schedules)
			.Select(g => new
			{
				g.Id,
				g.GameName,
				g.GameUrl,
				Schedules = g.Schedules.Select(s => new {
					Users = s.Users.Select(u => new {u.Id, u.UserName}),
					WeekDay = (int)s.DayOfWeek
				})
			}).ToListAsync();
		return Ok(games);
	}

	
	[HttpPost]
	public async Task<IActionResult> AddGame([FromBody] GameModel? game)
	{
		if (game == null)
			return BadRequest();
			
		dbContext.Games.Add(game);
			
		await dbContext.SaveChangesAsync();
		return Ok(game);
	}

	[HttpPut("{gameId}")]
	public async Task<IActionResult> UpdateGame(int gameId, [FromBody] GameModel game)
	{
		var existing = await dbContext.Games.FindAsync(gameId);
		if (existing == null) return NotFound();

		existing.GameUrl = game.GameUrl;
			
		await dbContext.SaveChangesAsync();
		return Ok(existing);
	}

	[HttpDelete("{gameId}")]
	public async Task<IActionResult> DeleteGame(int gameId)
	{
		var existing = await dbContext.Games.FindAsync(gameId);
		if (existing == null) return NotFound();

		dbContext.Games.Remove(existing);
		await dbContext.SaveChangesAsync();
		return NoContent();
	}
	

	[HttpPost("{gameId}/{weekDay}/assign")]
	public async Task<IActionResult> AssignUsers(int gameId, int weekDay, [FromBody] string[] userIds)
	{
		var game = await dbContext.Games
			.Include(g => g.Schedules)
			.ThenInclude(s => s.Users)
			.FirstOrDefaultAsync(g => g.Id == gameId);

		if (game == null)
			return NotFound();

		if (weekDay is < 0 or > 6)
			return BadRequest();

		var schedule = game.Schedules.FirstOrDefault(x => (int)x.DayOfWeek == weekDay);
			
		if (schedule == null)
		{
			schedule = new GameUserSchedule
			{
				DayOfWeek = (DayOfWeek)weekDay,
				GameId = gameId,
			};
				
			game.Schedules.Add(schedule);
		}
			
		var users = await dbContext.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
			
		schedule.Users.Clear();
		foreach (var user in users)
			schedule.Users.Add(user);

		await dbContext.SaveChangesAsync();

		return Ok();
	}
}