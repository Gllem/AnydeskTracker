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

	private async Task<IActionResult> SendCommandToAgent(string agentId, string command)
	{
		var agentState = AgentPresence.Get(agentId);
		if (agentState is null) 
			return NotFound(new { agentId, message = "Agent never connected" });
		if (!agentState.Online) 
			return Conflict(new { agentId, message = "Agent offline", lastSeenUtc = agentState.LastSeenUtc });

		return await SendCommand($"machine:{agentId}", command);
	}

	private async Task<IActionResult> SendCommandToAll(string command) => await SendCommand("agents:all", command);

	private async Task<IActionResult> SendCommand(string group, string command)
	{
		var cmd = new Command("cmd-" + Guid.NewGuid().ToString("N"), command);

		await hub.Clients.Group(group)
			.SendAsync("Command", cmd);

		return Ok(new
		{
			sent = group,
			commandId = cmd.CommandId
		});			
	}

#region KillApps

	[HttpGet("killApps/{agentId}")]
	public async Task<IActionResult> KillApps(string agentId) => await SendCommandToAgent(agentId, "KillMain");

	[HttpGet("killApps")]
	public async Task<IActionResult> KillApps() => await SendCommandToAll("KillMain");
#endregion

#region StartMain

	[HttpGet("startMain/{agentId}")]
	public async Task<IActionResult> StartMain(string agentId) => await SendCommandToAgent(agentId, "StartMain");


	[HttpGet("startMain")]
	public async Task<IActionResult> StartMain() => await SendCommandToAll("StartMain");

#endregion
}

public record Command(string CommandId, string Type);