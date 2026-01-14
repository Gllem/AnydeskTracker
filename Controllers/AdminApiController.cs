using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
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

namespace AnydeskTracker.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/admin")]
[ApiController]
public class AdminApiController(
	ApplicationDbContext context,
	PcService pcService,
	SheetsService sheetService,
	ParserService parserService)
	: ControllerBase
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

	[HttpPut("pcs/bulk-update")]
	public async Task<IActionResult> BulkUpdatePcs([FromBody] PcBulkUpdateDto[] updateDtos)
	{
		foreach (var updateDto in updateDtos)
		{
			var existing = await context.Pcs.FindAsync(updateDto.Id);
			if (existing == null)
				return NotFound();

			existing.PcId = updateDto.AnyDeskId;
			existing.BotId = updateDto.BotId;
			existing.Password = updateDto.Password;
			existing.SortOrder = updateDto.SortOrder;
		}

		await context.SaveChangesAsync();
		return Ok();
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
				
			var currentPcUsage = session == null ? null : context.PcUsages.Include(x => x.Pc).FirstOrDefault(pc => pc.IsActive && pc.WorkSessionId == session.Id);
				
			return new
			{
				UserId = user.Id,
				UserName = user.UserName ?? String.Empty,
				SessionStartTime = session?.StartTime.ToUtc(),
				CurrentPcId = currentPcUsage?.Pc.DisplayId,
				Paused = currentPcUsage?.IsPaused,
				PauseTime = currentPcUsage?.PauseStartTime?.ToUtc()
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

#region Bots

	[HttpGet("bots")]
	public async Task<IActionResult> GetAllBots()
	{
		var pcs = await context.Pcs.ToListAsync();
		return Ok(pcs.Select(pc =>
		{
			var lastAction = context.BotActions.Where(x => x.PcId == pc.Id).ToList().MaxBy(x => x.Timestamp);

			var dolphinChecks = context.DolphinActions.Where(x => x.PcId == pc.Id).ToList();
				
			var lastDolphinAction = dolphinChecks.MaxBy(x => x.Timestamp);
			var dolphinChecksCount = dolphinChecks.Count(x => x.Timestamp.Date == DateTime.UtcNow.Date);

			List<string> errorStatuses = new List<string> { };
				
			if (lastAction != null && (lastDolphinAction == null || lastDolphinAction.Timestamp < lastAction.Timestamp))
			{
				if(!string.IsNullOrEmpty(lastAction.ProcessesStatus))
					errorStatuses.Add("PRC");
				if(!string.IsNullOrEmpty(lastAction.SchedulerStatus))
					errorStatuses.Add("SC");
				if(!string.IsNullOrEmpty(lastAction.DiskStatus))
					errorStatuses.Add("DSK");
				if(!string.IsNullOrEmpty(lastAction.RamStatus))
					errorStatuses.Add("RAM");
				if(!string.IsNullOrEmpty(lastAction.UserStatus))
					errorStatuses.Add("USR");
			}

			string status = errorStatuses.Count > 0 ? "ERROR" : "OK";
				
			if (pc.Status == PcStatus.Busy)
				status = "BUSY";
			if (pc.Status == PcStatus.CoolingDown)
				status = "COOLING";
				
			return new
			{
				pc.BotId,
				pc.PcId,
				PcModelId = pc.Id,
				HasChecks = lastAction != null,
				ErrorStatuses = errorStatuses,
				Status = status,
				LastCheckTime = lastAction?.Timestamp.ToUtc(),
				LastDolphinCheckTime = lastDolphinAction?.Timestamp.ToUtc(),
				dolphinChecksCount
			};
		}));
	}
		
	[HttpPut("bot/{pcModelId}")]
	public async Task<IActionResult> UpdateBotInfo(int pcModelId, [FromBody]AdminBotUpdateDto updateDto)
	{
		var pc = await context.Pcs.FindAsync(pcModelId);
			
		if (pc == null)
			return NotFound("PC ID Not found");

		pc.BotId = updateDto.BotId;
		await context.SaveChangesAsync();

		return Ok();
	}
		
	[HttpGet("bot/{pcModelId}/actions/{date}")]
	public async Task<IActionResult> GetBotActions(int pcModelId, string date)
	{
		DateTime logsDateTime;
		try
		{
			int[] dateParse = date.Split(".").Select(int.Parse).ToArray();
			logsDateTime = new DateTime(dateParse[2], dateParse[1], dateParse[0]);
		}
		catch (Exception e)
		{
			return BadRequest($"Can't parse date: {e}");
		}
			
		var watchDogActions = await context.BotActions
			.Where(x => x.PcId == pcModelId && x.Timestamp.Date == logsDateTime)
			.OrderByDescending(x => x.Timestamp)
			.ToListAsync();

		var dolphinActions = await context.DolphinActions
			.Where(x => x.PcId == pcModelId && x.Timestamp.Date == logsDateTime)
			.OrderByDescending(x => x.Timestamp)
			.ToListAsync();
			
		return Ok(new
		{
			WatchDogActions = watchDogActions.Select(x => new
			{
				x.Error,
				Statuses = new Dictionary<string, string>
				{
					{"Выключенные процессы", x.ProcessesStatus},
					{"Выключенные задачи", x.SchedulerStatus},
					{"Диск", x.DiskStatus},
					{"Пользователь", x.UserStatus},
					{"Память", x.RamStatus}
				},
				timestamp = x.Timestamp.ToUtc()
			}),
			DolphinActions = dolphinActions.Select(x => new
			{
				Timestamp = x.Timestamp.ToUtc()
			})
		});
	}
#endregion

#region BotGames

	[HttpGet("bots/games")]
	public async Task<IActionResult> GetAllBotsGames()
	{
		var games = await context.BotGames.Where(x => x.IsGlobal).ToListAsync();
		return Ok(games);
	}
	
	[HttpGet("bots/games/{pcId}")]
	public async Task<IActionResult> GetOverrideBotGames(int pcId)
	{
		var pc = await context.Pcs
			.Include(x => x.OverrideBotGames)
			.ThenInclude(x => x.BotGame)
			.FirstOrDefaultAsync(x => x.Id == pcId);
		
		if (pc == null)
			return NotFound();
		
		return Ok(pc.OverrideBotGames.Select(x => x.BotGame));
	}
		
	[HttpPost("bots/games")]
	public async Task<IActionResult> AddBotGame([FromBody] BotGame? game)
	{
		if (game == null)
			return BadRequest();
			
		context.BotGames.Add(game);
			
		await context.SaveChangesAsync();
		return Ok(game);
	}
	
	[HttpPost("bots/games/{pcId}")]
	public async Task<IActionResult> AddBotGameOverride(int pcId, [FromBody] BotGame? game)
	{
		var pc = await context.Pcs
			.Include(x => x.OverrideBotGames)
			.ThenInclude(x => x.BotGame)
			.FirstOrDefaultAsync(x => x.Id == pcId);

		if (pc == null)
			return NotFound();
		
		if (game == null)
			return BadRequest();

		game.IsGlobal = false;

		context.BotGames.Add(game);
		
		await context.SaveChangesAsync();
		
		context.PcModelToBotGames.Add(new PcModelToBotGame
		{
			PcModelId = pc.Id,
			BotGameId = game.Id,
		});
			
		await context.SaveChangesAsync();
		return Ok(game);
	}
		
	[HttpDelete("bots/games/{id}")]
	public async Task<IActionResult> DeleteBotGame(int id)
	{
		var botGame = await context.BotGames.FindAsync(id);
			
		if (botGame == null) 
			return NotFound();

		var pcModelToBotGame = await context.PcModelToBotGames.FirstOrDefaultAsync(x => x.BotGameId == id);

		if (pcModelToBotGame != null)
		{
			context.PcModelToBotGames.Remove(pcModelToBotGame);
		}
		
		context.BotGames.Remove(botGame);
		
		await context.SaveChangesAsync();
		return NoContent();
	}
#endregion		
		
#region Other

	[HttpPost("fetchCredentials")]
	public async Task<IActionResult> FetchBlockedCredentials()
	{
		await parserService.FetchBlockedCredentials();
		return Ok();
	}

#endregion
}