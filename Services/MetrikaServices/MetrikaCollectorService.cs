using AnydeskTracker.Data;
using AnydeskTracker.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AnydeskTracker.Services.MetrikaServices;

public class MetrikaCollectorService(YandexMetrikaService yandexMetrikaService,
	ApplicationDbContext dbContext,
	AgentCommandsService agentCommandsService)
{
	private static Dictionary<string, string> StartedJobIds = new Dictionary<string, string>();
	
	public void StartCollectorJob(string botId, string userId, int browserId, bool firstCheck)
	{
		if (StartedJobIds.TryGetValue(botId, out string? prevJobId))
		{
			BackgroundJob.Delete(prevJobId);
		}
		
		var jobId = BackgroundJob.Schedule<MetrikaCollectorService>(
			x => x.RefreshGameRevenueJob(botId, userId, browserId, firstCheck),
			TimeSpan.FromMinutes(10)
		);

		StartedJobIds[botId] = jobId;
	}

	public void StopCollectorJob(string botId)
	{
		if (StartedJobIds.TryGetValue(botId, out string? prevJobId))
		{
			BackgroundJob.Delete(prevJobId);
		}
	}
	
	public async Task RefreshGameRevenueJob(string botId, string userId, int browserId, bool firstCheck)
	{
		var browserModel = await dbContext.BrowserRevenues.FindAsync(browserId);
		var agentPresent = agentCommandsService.IsAgentOnline(botId);
        
		if(browserModel == null)
			return;

		if (!agentPresent)
		{
			StartCollectorJob(botId, userId, browserId, firstCheck);
			return;
		}

		await yandexMetrikaService.GetCurrentBrowserRevenue(browserModel.Browser);

		var collectionSettings = await dbContext.MetrikaCollectionSettings.FirstAsync();
		var rewardThreshold = collectionSettings.RevenueThreshold;
		
		if(false)
		// if (browserModel.DeltaRevenue > rewardThreshold)
		{
			StartCollectorJob(botId, userId, browserId, false);
			if (firstCheck)
				await agentCommandsService.SendCommandToAgent(botId, "SuccessGameRevenueCheck");

			await LogUserAgentAction(userId, browserModel.Browser, true, browserModel.DeltaRevenue);
		}
		else
		{
			await agentCommandsService.SendCommandToAgent(botId, "FailedGameRevenueCheck");
			await LogUserAgentAction(userId, browserModel.Browser, false, browserModel.DeltaRevenue);
		}
		
		await dbContext.SaveChangesAsync();
	}

	private async Task LogUserAgentAction(string userId, string browser, bool success, decimal deltaRevenue)
	{
		var pcUsage = await dbContext.PcUsages.FirstOrDefaultAsync(x => x.IsActive && x.WorkSession.UserId == userId);
		if(pcUsage == null)
			return;

		dbContext.UserAgentActions.Add(new UserAgentAction
		{
			UserId = userId,
			WorkSessionId = pcUsage.WorkSessionId,
			PcId = pcUsage.PcId,
			UserLogType = UserLogType.RevenueCheck,
			AdditionalParams = JsonConvert.SerializeObject(new
			{
				browser,
				success,
				deltaRevenue
			})
		});
	}
}