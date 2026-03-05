using AnydeskTracker.Data;
using AnydeskTracker.DTOs;
using AnydeskTracker.Extensions;
using AnydeskTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AnydeskTracker.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/admin/users")]
[ApiController]
public class AdminApiUsersController(ApplicationDbContext dbContext) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetAllUsers()
	{
		var users = await dbContext.Users.ToListAsync();
		return Ok(users.Select(user =>
		{
			var session = dbContext.WorkSessionModels.FirstOrDefault(workSession => workSession.IsActive && workSession.UserId == user.Id);
				
			var currentPcUsage = session == null ? null : dbContext.PcUsages.Include(x => x.Pc).FirstOrDefault(pc => pc.IsActive && pc.WorkSessionId == session.Id);
				
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
		var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

		if (user == null)
			return null;

		var session = await dbContext.WorkSessionModels.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == sessionId);

		return session;
	}

	[HttpPut("{userId}")]
	public async Task<IActionResult> UpdateUserInfo(string userId, [FromBody]AdminUserUpdateDto updateDto)
	{
		var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
			
		if (user == null)
			return NotFound("User ID Not found");

		user.UserName = updateDto.UserName;

		await dbContext.SaveChangesAsync();

		return Ok();
	}

	[HttpGet("{userId}/{sessionId:int}")]
	public async Task<IActionResult> GetUserSession(string userId, int sessionId)
	{
		var session = await GetSession(userId, sessionId);

		if (session == null)
			return NotFound();
			
		return Ok(session);
	}

	[HttpGet("{userId}/{sessionId:int}/actions")]
	public async Task<IActionResult> GetUserSessionActions(string userId, int sessionId)
	{
		var session = await GetSession(userId, sessionId);

		if (session == null)
			return NotFound();

		var actions = await dbContext.UserActions.Where(x => x.UserId == userId && x.WorkSessionId == sessionId).ToListAsync();

		return Ok(actions.Select(x => new
		{
			actionType = x.ActionTypeText,
			description = x.Description ?? String.Empty,
			timestamp = x.Timestamp.ToUtc()
		}));
	}

	[HttpGet("{userId}/{sessionId:int}/agentActions")]
	public async Task<IActionResult> GetUserSessionAgentActions(string userId, int sessionId)
	{
		var session = await GetSession(userId, sessionId);

		if (session == null)
			return NotFound();
		
		var games = await dbContext.GameCatalog.ToListAsync();
		
		var agentActions = (await dbContext.UserAgentActions
			.Where(x => x.UserId == userId && x.WorkSessionId == sessionId)
			.Include(userAgentAction => userAgentAction.PcModel).ToListAsync()).Select(x =>
		{
			var additionalParams = JsonConvert.DeserializeAnonymousType(x.AdditionalParams,
				new
				{
					browser = "", 
					gameId = -1
				});

			var game = games.FirstOrDefault(g => g.Id == additionalParams?.gameId);
			
			return new
			{
				LogType = x.UserLogType,
				LogTypeText = x.UserLogType.GetDisplayName(),
				Browser = additionalParams?.browser ?? "",
				GameId = game?.Id ?? -1,
				GameName = game?.Name ?? "",
				GameUrl = game?.Url ?? "",
				Timestamp = x.Timestamp.ToUtc(),
				Pc = x.PcModel.DisplayId
			};
		}).Reverse().ToList();

		var browsersCount = agentActions
			.Where(x => x.LogType == UserLogType.BrowserOpen)
			.GroupBy(x => x.Browser)
			.Select(x => new
			{
				Name = x.Key,
				Count = x.Count()
			});

		var gamesCount = 
			agentActions.Where(x => 
					x.LogType is UserLogType.BrowserOpen or UserLogType.GameSelectionUpdated 
					&& x.GameId != -1)
			.GroupBy(x => x.GameId)
			.Select(x => new
			{
				games.First(g => g.Id == x.Key).Name,
				Count = x.Count()
			});
		
		return Ok(new
		{
			browsers = browsersCount.OrderByDescending(x => x.Count),
			
			games = gamesCount.OrderByDescending(x => x.Count),
			
			agentActions,
		});
	}
}