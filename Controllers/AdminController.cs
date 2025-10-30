using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnydeskTracker.Controllers;


[Authorize(Roles = "Admin")]
[Route("Admin")]
public class AdminController : Controller
{
	[HttpGet]
	public async Task<IActionResult> Index()
	{
		return View("AdminIndex");
	}
	
	[HttpGet("Computers")]
	public async Task<IActionResult> Computers()
	{
		return View("Computers");
	}
	
	[HttpGet("Games")]
	public async Task<IActionResult> Games()
	{
		return View("Games");
	}
}