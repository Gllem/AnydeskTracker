using AnydeskTracker.Data;
using AnydeskTracker.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AnydeskTracker.Services.MetrikaServices;

public class MetrikaCollectorService(YandexMetrikaService yandexMetrikaService,
	ApplicationDbContext dbContext,
	IBackgroundJobClient jobs,
	AgentCommandsService agentCommandsService)
{
	private const decimal DeltaRewardThreshold = 100;

	private static Dictionary<string, string> StartedJobIds = new Dictionary<string, string>();
	
	public void StartCollectorJob(string botId, string userId, int browserId, bool firstCheck)
	{
		if (StartedJobIds.TryGetValue(botId, out string? prevJobId))
		{
			BackgroundJob.Delete(prevJobId);
		}
		
		var jobId = BackgroundJob.Schedule<MetrikaCollectorService>(
			x => x.RefreshGameRevenueJob(botId, userId, browserId, firstCheck),
			TimeSpan.FromSeconds(5)
		);

		StartedJobIds[botId] = jobId;
	}
	
	public async Task RefreshGameRevenueJob(string botId, string userId, int browserId, bool firstCheck)
	{
		var browserModel = await dbContext.BrowserRevenues.FindAsync(browserId);
        
		if(browserModel == null)
			return;

		await yandexMetrikaService.GetCurrentBrowserRevenue(browserModel.Browser);

		if (browserModel.DeltaRevenue > DeltaRewardThreshold)
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