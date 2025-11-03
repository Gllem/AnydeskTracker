using AnydeskTracker.Data;
using AnydeskTracker.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnydeskTracker.Controllers;


[Authorize(Roles = "Admin")]
[Route("Admin")]
public class AdminController(ApplicationDbContext context) : Controller
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

	[HttpGet("Users")]
	public async Task<IActionResult> Users()
	{
		return View("Users");
	}

	[HttpGet("User/{userId}")]
	public async Task<IActionResult> GetUser(string userId)
	{
		var user = await context.Users.FindAsync(userId);
		
		if (user == null)
			return NotFound();
		
		var sessions = context.WorkSessionModels.Where(x => x.User.Id == userId).ToArray();

		return View("User", new UserDto(user, sessions));
	}
}