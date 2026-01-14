using AnydeskTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnydeskTracker.Controllers;

[Route("api")]
[ApiController]
public class PublicApiController(PcService pcService) : ControllerBase
{
	[HttpGet("pcs")]
	public async Task<IActionResult> GetAllPcs()
	{
		var pcs = await pcService.GetAllPcsNonSensitive();
		return Ok(pcs);
	}
}