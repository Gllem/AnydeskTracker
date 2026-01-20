using Microsoft.AspNetCore.SignalR;

public class AgentHub : Hub
{
	public Task Register(string agentId) =>
		Groups.AddToGroupAsync(Context.ConnectionId, $"machine:{agentId}");

	public Task CommandResult(CommandResult res)
	{
		Console.WriteLine($"[{res.AgentId}] {res.CommandId} ok={res.Ok} msg={res.Message}");
		return Task.CompletedTask;
	}
}

public record CommandResult(string CommandId, string AgentId, bool Ok, string Message);