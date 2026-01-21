using AnydeskTracker.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AnydeskTracker.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/admin/agents")]
[ApiController]
public class AdminApiAgentController(IHubContext<AgentHub> hub) : ControllerBase
{
	[HttpGet("{agentId}")]
	public async Task<IActionResult> GetAgentStatus(string agentId)
	{
		var agentState = AgentPresence.Get(agentId);
		
		if (agentState is null)
			return NotFound();	
		
		return Ok(new
		{
			online = agentState.Online,
			lastSeenUtc = agentState.LastSeenUtc.ToUtc()
		});
	}
	
	[HttpGet("{agentId}/killApps")]
	public async Task<IActionResult> KillApps(string agentId)
	{
		var agentState = AgentPresence.Get(agentId);
		if (agentState is null) 
			return NotFound(new { agentId, message = "Agent never connected" });
		if (!agentState.Online) 
			return Conflict(new { agentId, message = "Agent offline", lastSeenUtc = agentState.LastSeenUtc });
		
		var cmd = new Command("cmd-" + Guid.NewGuid().ToString("N"), "KillMain");
		await hub.Clients.Group($"machine:{agentId}").SendAsync("Command", cmd);
		return Ok(cmd);
	}

	[HttpGet("killAppsAllBots")]
	public async Task<IActionResult> KillAppsAllBots()
	{
		var cmd = new Command("cmd-" + Guid.NewGuid().ToString("N"), "KillMain");

		await hub.Clients.Group("agents:all")
			.SendAsync("Command", cmd);

		return Ok(new
		{
			sent = "agents:all",
			commandId = cmd.CommandId
		});
	}
}

public record Command(string CommandId, string Type);