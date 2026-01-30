using System.Net;
using AnydeskTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace AnydeskTracker.Services;

public class AgentGamesUpdater(AgentCommandsService agentCommandsService, ApplicationDbContext dbContext)
{
	public async Task<(HttpStatusCode code, object payload)> UpdateGamesDefault()
	{
		var eligibleIds = dbContext.Pcs.Include(x => x.OverridenBotGames)
			.Where(x => x.OverridenBotGames.Count == 0)
			.Select(x => x.BotId)
			.ToArray();
		
		var onlineEligibleGroups = eligibleIds
			.Where(id => AgentPresence.Get(id)?.Online == true)
			.Select(id => $"machine:{id}")
			.ToArray();
		
		
		return await agentCommandsService.SendCommand(onlineEligibleGroups, "DownloadGames");
	}
	
	public async Task<(HttpStatusCode code, object payload)> UpdateGamesOverride(string agentId)
	{
		return await agentCommandsService.SendCommandToAgent(agentId, "DownloadGames");
	}
}