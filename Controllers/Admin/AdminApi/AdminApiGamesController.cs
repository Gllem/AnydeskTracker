using AnydeskTracker.Data;
using AnydeskTracker.DTOs;
using AnydeskTracker.Extensions;
using AnydeskTracker.Models;
using AnydeskTracker.Models.Game;
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
		var games = await dbContext.GameCatalog
			.Include(g => g.UserSchedules)
			.ThenInclude(s => s.UserLinks)
			.ThenInclude(u => u.User)
			.Select(g => new
			{
				g.Id,
				g.Name,
				g.Url,
				g.YandexMetrikaId,
				g.AccountName,
				Schedules = g.UserSchedules.Select(s => new {
					Users = s.UserLinks.Select(u => new {u.User.Id, u.User.UserName}),
					WeekDay = (int)s.DayOfWeek
				})
			}).ToListAsync();
		return Ok(games);
	}
	
	[HttpPost]
	public async Task<IActionResult> AddGame([FromBody] Game? game)
	{
		if (game == null)
			return BadRequest();

		game.Url = game.Url.NormalizeUrl();
		
		var existingGame = await dbContext.GameCatalog.FirstOrDefaultAsync(x => x.Url == game.Url);

		if (existingGame != null)
			return Conflict();
			
		dbContext.GameCatalog.Add(game);
			
		await dbContext.SaveChangesAsync();
		return Ok(game);
	}
	
	[HttpDelete("{gameId}")]
	public async Task<IActionResult> DeleteGame(int gameId)
	{
		var existing = await dbContext.GameCatalog.FindAsync(gameId);
		if (existing == null) return NotFound();

		dbContext.GameCatalog.Remove(existing);
		await dbContext.SaveChangesAsync();
		return NoContent();
	}
	
	[HttpPut("bulk-update")]
	public async Task<IActionResult> BulkUpdateGames([FromBody] GameDto[] updateDtos)
	{
		foreach (var updateDto in updateDtos)
		{
			var existing = await dbContext.GameCatalog.FindAsync(updateDto.Id);
			if (existing == null)
				return NotFound();

			existing.Name = updateDto.Name;
			existing.Url = updateDto.Url.NormalizeUrl();
			existing.YandexMetrikaId = updateDto.YandexMetrikaId;
			existing.AccountName = updateDto.AccountName;
		}

		await dbContext.SaveChangesAsync();
		return Ok();
	}

	[HttpPost("{gameId}/{weekDay}/assign")]
	public async Task<IActionResult> AssignUsers(int gameId, int weekDay, [FromBody] string[] userIds)
	{
		var game = await dbContext.GameCatalog
			.Include(g => g.UserSchedules)
			.ThenInclude(s => s.UserLinks)
			.FirstOrDefaultAsync(g => g.Id == gameId);

		if (game == null)
			return NotFound();

		if (weekDay is < 0 or > 6)
			return BadRequest();

		var schedule = game.UserSchedules.FirstOrDefault(x => (int)x.DayOfWeek == weekDay);
			
		if (schedule == null)
		{
			schedule = new GameSchedule
			{
				DayOfWeek = (DayOfWeek)weekDay,
				GameId = gameId,
			};
				
			game.UserSchedules.Add(schedule);
		}
			
		var users = await dbContext.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
			
		schedule.UserLinks.Clear();
		foreach (var user in users)
			schedule.UserLinks.Add(new GameUserScheduleToUser
			{
				User = user
			});

		await dbContext.SaveChangesAsync();

		return Ok();
	}
}