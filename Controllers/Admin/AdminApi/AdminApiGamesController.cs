using AnydeskTracker.Data;
using AnydeskTracker.Models;
using AnydeskTracker.Models.GameRefactor;
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
				Schedules = g.UserSchedules.Select(s => new {
					Users = s.UserLinks.Select(u => new {u.User.Id, u.User.UserName}),
					WeekDay = (int)s.DayOfWeek
				})
			}).ToListAsync();
		return Ok(games);
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