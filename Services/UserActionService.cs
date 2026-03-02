using System;
using System.Threading.Tasks;
using AnydeskTracker.Data;
using AnydeskTracker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AnydeskTracker.Services
{
	public class UserActionService(ApplicationDbContext context, PcService pcService)
	{
		public async Task LogAsync(WorkSessionModel workSession, ActionType actionType, string? description = null)
		{
			var action = new UserAction
			{
				UserId = workSession.UserId,
				WorkSessionId = workSession.Id,
				ActionType = actionType,
				Description = description,
				Timestamp = DateTime.UtcNow
			};

			context.UserActions.Add(action);
			await context.SaveChangesAsync();
		}
		
		public async Task LogFromAgentAsync(string botId, UserLog? userLog)
		{
			if(userLog == null)
				return;

			var pcUsage = await pcService.GetPcUsageFromBotId(botId);
			
			if(pcUsage == null)
				return;

			var action = new UserAgentAction
			{
				UserId = pcUsage.WorkSession.UserId,
				WorkSessionId = pcUsage.WorkSessionId,
				PcId = pcUsage.PcId,
				UserLogType = userLog.LogType,
				AdditionalParams = userLog.AdditionalParams ?? "",
			};

			context.UserAgentActions.Add(action);

			await context.SaveChangesAsync();
		}
	}
}