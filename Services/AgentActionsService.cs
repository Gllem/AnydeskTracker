using AnydeskTracker.Data;
using AnydeskTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace AnydeskTracker.Services;

public class AgentActionsService(ApplicationDbContext dbContext)
{
	public async Task SetAhkErrorState(string botId, bool state)
	{
		var pc = await dbContext.Pcs.FirstOrDefaultAsync(x => x.BotId == botId);

		if(pc == null)
			return;
		
		pc.AhkError = state;

		await dbContext.SaveChangesAsync();
	}
}