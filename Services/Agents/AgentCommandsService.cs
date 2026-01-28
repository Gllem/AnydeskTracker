using System.Net;
using AnydeskTracker.Controllers;
using Microsoft.AspNetCore.SignalR;

namespace AnydeskTracker.Services;

public class AgentCommandsService(IHubContext<AgentHub> hub)
{
	public async Task<(HttpStatusCode code, object payload)> SendCommandToAgent(string agentId, string command)
	{
		var agentState = AgentPresence.Get(agentId);
		if (agentState is null)
			return (HttpStatusCode.NotFound, "Agent never connected");
		if (!agentState.Online)
			return (HttpStatusCode.Conflict, "Agent offline");
		
		return await SendCommand($"machine:{agentId}", command);
	}

	public async Task<(HttpStatusCode code, object payload)> SendCommandToAll(string command) => await SendCommand("agents:all", command);

	public async Task<(HttpStatusCode code, object payload)> SendCommand(string group, string command)
	{
		var cmd = new Command("cmd-" + Guid.NewGuid().ToString("N"), command);

		await hub.Clients.Group(group)
			.SendAsync("Command", cmd);

		return (HttpStatusCode.OK, new
		{
			sent = group,
			commandId = cmd.CommandId
		});
	}

	public async Task<(HttpStatusCode code, object payload)> SendCommand(string[] groups, string command)
	{
		var cmd = new Command("cmd-" + Guid.NewGuid().ToString("N"), command);

		await hub.Clients.Groups(groups)
			.SendAsync("Command", cmd);

		return (HttpStatusCode.OK, new
		{
			sent = groups,
			commandId = cmd.CommandId
		});
	}
}