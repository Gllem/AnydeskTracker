using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AnydeskTracker.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/admin/agents")]
[ApiController]
public class AdminApiAgentController(IHubContext<AgentHub> hub) : ControllerBase
{
	[HttpGet("{agentId}/killApps")]
	public async Task<IActionResult> KillApps(string agentId)
	{
		var cmd = new Command("cmd-" + Guid.NewGuid().ToString("N"), "KillApps");
		await hub.Clients.Group($"machine:{agentId}").SendAsync("Command", cmd);
		return Ok(cmd);
	}
}

public record Command(string CommandId, string Type);