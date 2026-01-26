using AnydeskTracker.Models;
using AnydeskTracker.Services;
using Microsoft.AspNetCore.SignalR;

public class AgentHub(AgentActionsService actionsService) : Hub
{
	public async Task Register(string agentId)
	{
		Context.Items["agentId"] = agentId;
		AgentPresence.SetOnline(agentId);
		
		await Groups.AddToGroupAsync(Context.ConnectionId, $"machine:{agentId}");
		await Groups.AddToGroupAsync(Context.ConnectionId, "agents:all");
	}

	public Task CommandResult(CommandResult res)
	{
		Console.WriteLine($"[{res.AgentId}] {res.CommandId} ok={res.Ok} msg={res.Message}");
		AgentPresence.Touch(res.AgentId);
		return Task.CompletedTask;
	}
	
	public async Task AgentEvent(AgentEventDto ev)
    {
	    AgentPresence.Touch(ev.AgentId);

	    switch (ev.Type)
	    {
		    case "AhkError":
			    await actionsService.LogAction(AgentActionType.Error, "AHK", "Ошибка AHK");
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

public record CommandResult(string CommandId, string AgentId, bool Ok, string Message);
public record AgentEventDto(string AgentId, string Type, string Message, DateTime TimeUtc);