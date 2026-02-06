using System.Text.Json;
using AnydeskTracker.Data;
using AnydeskTracker.DTOs;
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

	public async Task LogLastScheduleStatus(string botId, string status)
	{
		var pc = await dbContext.Pcs.FirstOrDefaultAsync(x => x.BotId == botId);
		
		if(pc == null)
			return;

		var log = JsonSerializer.Deserialize<BotScheduleLogDto>(status);

		if(log == null)
			return;


		pc.LastActiveSchedule = log.JobSchedule;
		pc.LastStatus = log.CurrentStatus;
		pc.NextLaunchTime = log.NextLaunchTime;
		
		switch (log.CurrentStatus)
		{
			case BotScheduleStatus.Working:
				pc.LastLaunchTime = log.Time;
				break;
		}

		await dbContext.SaveChangesAsync();
	}
}