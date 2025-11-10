using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnydeskTracker.Data;
using AnydeskTracker.DTOs;
using AnydeskTracker.Extensions;
using AnydeskTracker.Models;
using AnydeskTracker.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using HtmlAgilityPack;

namespace AnydeskTracker.Controllers
{
	[Authorize(Roles = "Admin")]
	[Route("api/admin")]
	[ApiController]
	public class AdminApiController(ApplicationDbContext context, PcService pcService, SheetsService sheetService) : ControllerBase
	{
#region Pcs
		[HttpGet("pcs")]
		public async Task<IActionResult> GetAllPcs()
		{
			var pcs = await pcService.GetAllPcs();
			return Ok(pcs);
		}

		[HttpPost("pcs")]
		public async Task<IActionResult> AddPc([FromBody] PcModel? pc)
		{
			pc.LastStatusChange = DateTime.UtcNow;
			context.Pcs.Add(pc);
			await context.SaveChangesAsync();
			return Ok(pc);
		}

		[HttpPost("pcs/updatePcs")]
		public async Task<IActionResult> UpdatePcsFromSheet([FromBody] AdminGoogleSheetDto sheetDto)
		{
			var range = $"{sheetDto.SheetName}!A1:B";
			var request = sheetService.Spreadsheets.Values.Get(sheetDto.SheetId, range);

			try
			{
				ValueRange response = await request.ExecuteAsync();

				if (response == null || response.Values == null)
					return StatusCode(StatusCodes.Status500InternalServerError);

				var table = response.Values;

				for (int i = 1; i < table.Count; i++)
				{
					var row = table[i];
					var rowLength = row.Count;
					
					if(rowLength <= 1 || row[0] == null || string.IsNullOrWhiteSpace(row[0].ToString()))
						continue;

					string anydeskPcId = row[0].ToString() ?? "";
					string password = row[1].ToString() ?? "";

					var pc = await context.Pcs.FirstOrDefaultAsync(x => x != null && x.PcId == anydeskPcId);

					if(pc == null)
						continue;
					
					pc.Password = password;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}

			await context.SaveChangesAsync();
			return Ok();
		}

		[HttpPut("pcs/{id}")]
		public async Task<IActionResult> UpdatePc(int id, [FromBody] PcModel pc)
		{
			var existing = await context.Pcs.FindAsync(id);
			if (existing == null) return NotFound();

			existing.PcId = pc.PcId;
			existing.Password = pc.Password;
			existing.Status = pc.Status;
			existing.LastStatusChange = DateTime.UtcNow;

			await context.SaveChangesAsync();
			return Ok(existing);
		}

		[HttpPut("pcs/{id}/forceFreeUp")]
		public async Task<IActionResult> ForceFreeUpPc(int id)
		{
			var pc = await context.Pcs.FindAsync(id);
			
			if (pc == null)
				return NotFound();
			
			if (pc.Status == PcStatus.Free)
				return BadRequest();
			
			pc.Status = PcStatus.Free;
			pc.LastStatusChange = DateTime.UtcNow;

			var usage = await context.PcUsages.FirstOrDefaultAsync(x => x.IsActive && x.PcId == id);

			if (usage == null)
			{
				await context.SaveChangesAsync();
				return Ok("No Usage");
			}
			
			usage.IsActive = false;
			usage.EndTime = DateTime.UtcNow;
			
			await context.SaveChangesAsync();
			return Ok();
		}

		[HttpDelete("pcs/{id}")]
		public async Task<IActionResult> DeletePc(int id)
		{
			var existing = await context.Pcs.FindAsync(id);
			if (existing == null) return NotFound();

			context.Pcs.Remove(existing);
			await context.SaveChangesAsync();
			return NoContent();
		}
#endregion

#region Games
		[HttpGet("games")]
		public async Task<IActionResult> GetAllGames()
		{
			var games = await context.Games
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

		[HttpPost("games")]
		public async Task<IActionResult> AddGame([FromBody] GameModel? game)
		{
			if (game == null)
				return BadRequest();
			
			context.Games.Add(game);
			
			await context.SaveChangesAsync();
			return Ok(game);
		}


		[HttpPut("games/{id}")]
		public async Task<IActionResult> UpdateGame(int id, [FromBody] GameModel game)
		{
			var existing = await context.Games.FindAsync(id);
			if (existing == null) return NotFound();

			existing.GameUrl = game.GameUrl;
			
			await context.SaveChangesAsync();
			return Ok(existing);
		}

		[HttpDelete("games/{id}")]
		public async Task<IActionResult> DeleteGame(int id)
		{
			var existing = await context.Games.FindAsync(id);
			if (existing == null) return NotFound();

			context.Games.Remove(existing);
			await context.SaveChangesAsync();
			return NoContent();
		}

		[HttpPost("games/{gameId}/{weekDay}/assign")]
		public async Task<IActionResult> AssignUsers(int gameId, int weekDay, [FromBody] string[] userIds)
		{
			var game = await context.Games
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
			
			var users = await context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
			
			schedule.Users.Clear();
			foreach (var user in users)
				schedule.Users.Add(user);

			await context.SaveChangesAsync();

			return Ok();
		}
#endregion

#region Users
		[HttpGet("users")]
		public async Task<IActionResult> GetAllUsers()
		{
			var users = await context.Users.ToListAsync();
			return Ok(users.Select(user =>
			{
				var session = context.WorkSessionModels.FirstOrDefault(workSession => workSession.IsActive && workSession.UserId == user.Id);
				
				var currentPcUsage = session == null ? null : context.PcUsages.FirstOrDefault(pc => pc.IsActive && pc.WorkSessionId == session.Id);
				
				return new
				{
					UserId = user.Id,
					UserName = user.UserName ?? String.Empty,
					SessionStartTime = session?.StartTime.ToUtc(),
					CurrentPcId = currentPcUsage?.PcId
				};
			}));
		}

		private async Task<WorkSessionModel?> GetSession(string userId, int sessionId)
		{
			var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);

			if (user == null)
				return null;

			var session = await context.WorkSessionModels.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == sessionId);

			return session;
		}

		[HttpPut("user/{userId}")]
		public async Task<IActionResult> UpdateUserInfo(string userId, [FromBody]AdminUserUpdateDto updateDto)
		{
			var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
			
			if (user == null)
				return NotFound("User ID Not found");

			user.UserName = updateDto.UserName;

			await context.SaveChangesAsync();

			return Ok();
		}

		[HttpGet("user/{userId}/{sessionId:int}")]
		public async Task<IActionResult> GetUserSession(string userId, int sessionId)
		{
			var session = await GetSession(userId, sessionId);

			if (session == null)
				return NotFound();
			
			return Ok(session);
		}

		[HttpGet("user/{userId}/{sessionId:int}/actions")]
		public async Task<IActionResult> GetUserSessionActions(string userId, int sessionId)
		{
			var session = await GetSession(userId, sessionId);

			if (session == null)
				return NotFound();

			var actions = await context.UserActions.Where(x => x.UserId == userId && x.WorkSessionId == sessionId).ToListAsync();

			return Ok(actions.Select(x => new
			{
				actionType = x.ActionTypeText,
				description = x.Description ?? String.Empty,
				timestamp = x.Timestamp.ToUtc()
			}));
		}
#endregion
	}
}