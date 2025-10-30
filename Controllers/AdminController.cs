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
	public async Task<IActionResult> User(string userId)
	{
		return View("User", new UserDto(context, userId));
	}
}