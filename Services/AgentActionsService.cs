using AnydeskTracker.Data;
using AnydeskTracker.Models;

namespace AnydeskTracker.Services;

public class AgentActionsService(ApplicationDbContext dbContext)
{
	public async Task LogAction(AgentActionType actionType, string actionName, string actionDescription)
	{
		var action = new PcAgentAction
		{
			ActionType = actionType,
			Name = actionName,
			Description = actionDescription
		};

		dbContext.AgentActions.Add(action);
		await dbContext.SaveChangesAsync();
	}
}