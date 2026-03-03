using AnydeskTracker.Extensions;
using AnydeskTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnydeskTracker.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/admin/agents")]
[ApiController]
public class AdminApiAgentController(AgentCommandsService agentCommandsService) : ControllerBase
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

#region KillApps

	[HttpGet("killApps/{agentId}")]
	public async Task<IActionResult> KillApps(string agentId)
	{
		var (code, payload) = await agentCommandsService.SendCommandToAgent(agentId, "KillMain");
		return StatusCode((int) code, payload);
	}

	[HttpGet("killApps")]
	public async Task<IActionResult> KillApps()
	{
		var (code, payload) = await agentCommandsService.SendCommandToAll("KillMain");
		return StatusCode((int)code, payload);
	}

#endregion

#region StartMain
	[HttpGet("startMain/{agentId}")]
	public async Task<IActionResult> StartMain(string agentId)
	{
		var (code, payload) = await agentCommandsService.SendCommandToAgent(agentId, "StartMain");
		return StatusCode((int)code, payload);
	}


	[HttpGet("startMain")]
	public async Task<IActionResult> StartMain()
	{
		var (code, payload) = await agentCommandsService.SendCommandToAll("StartMain");
		return StatusCode((int)code, payload);
	}
#endregion
	
#region RestartMain
	[HttpGet("restartMain/{agentId}")]
	public async Task<IActionResult> RestartMain(string agentId)
	{
		var (code, payload) = await agentCommandsService.SendCommandToAgent(agentId, "RestartMain");
		return StatusCode((int)code, payload);
	}


	[HttpGet("restartMain")]
	public async Task<IActionResult> RestartMain()
	{
		var (code, payload) = await agentCommandsService.SendCommandToAll("RestartMain");
		return StatusCode((int)code, payload);
	}
#endregion
	
	[HttpGet("updateAll")]
	public async Task<IActionResult> UpdateAll()
	{
		var (code, payload) = await agentCommandsService.SendCommandToAll("CheckUpdate");
		return StatusCode((int)code, payload);
	}
}

public record Command(string CommandId, string Type);