using AnydeskTracker.Models;
using AnydeskTracker.Services;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

public class AgentHub(AgentActionsService actionsService, UserActionService userActionService) : Hub
{
	public async Task Register(string agentId)
	{
		Context.Items["agentId"] = agentId;
		AgentPresence.SetOnline(agentId);
		
		await Groups.AddToGroupAsync(Context.ConnectionId, $"machine:{agentId}");
		await Groups.AddToGroupAsync(Context.ConnectionId, "agents:all");
	}

	
	public async Task AgentEvent(AgentEventDto ev)
    {
	    AgentPresence.Touch(ev.AgentId);
	    
	    switch (ev.Type)
	    {
		    case "AhkError":
			    await actionsService.SetAhkErrorState(ev.AgentId, true);
			    break;
		    case "AhkOk":
			    await actionsService.SetAhkErrorState(ev.AgentId, false);
			    break;
		    case "ScheduleStatus":
			    await actionsService.LogLastScheduleStatus(ev.AgentId, ev.Message);
			    break;
		    case "UserLog":
			    await userActionService.LogFromAgentAsync(ev.AgentId, JsonConvert.DeserializeObject<UserLog>(ev.Message));
			    break;
		    default:
			    return;
	    }
    }
	
	public override Task OnDisconnectedAsync(Exception? exception)
	{
		if (Context.Items.TryGetValue("agentId", out var id) && id is string agentId)
			AgentPresence.SetOffline(agentId);

		return base.OnDisconnectedAsync(exception);
	}
}

public record AgentEventDto(string AgentId, string Type, string Message, DateTime TimeUtc);
public record UserLog(UserLogType LogType, string? AdditionalParams);