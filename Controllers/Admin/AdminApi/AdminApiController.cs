using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AnydeskTracker.Services;
namespace AnydeskTracker.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/admin")]
[ApiController]
public class AdminApiController(ParserService parserService) : ControllerBase
{
	[HttpPost("fetchCredentials")]
	public async Task<IActionResult> FetchBlockedCredentials()
	{
		await parserService.FetchBlockedCredentials();
		return Ok();
	}
}