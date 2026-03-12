using System;
using System.Threading.Tasks;
using AnydeskTracker.Data;
using AnydeskTracker.Models;
using AnydeskTracker.Services.MetrikaServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AnydeskTracker.Services
{
	public class UserActionService(ApplicationDbContext context, PcService pcService, YandexMetrikaService yandexMetrikaService, MetrikaCollectorService metrikaCollectorService)
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
			
			switch (userLog.LogType)
			{
				case UserLogType.WindowOpen:
					metrikaCollectorService.StopCollectorJob(botId);
					break;
				case UserLogType.BrowserOpen:
					var parsedParams = 
						JsonConvert.DeserializeAnonymousType(userLog.AdditionalParams ?? "", new {browser = ""});
					
					if(parsedParams == null)
						break;

					var browserModel = await context.BrowserRevenues.FirstOrDefaultAsync(x => x.Browser == parsedParams.browser);

					if(browserModel == null)
						break;
					
					await yandexMetrikaService.GetCurrentBrowserRevenue(browserModel.Browser);

					metrikaCollectorService.StartCollectorJob(botId, pcUsage.WorkSession.UserId, browserModel.Id, true);
					
					break;
			}
		}
	}
}