using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnydeskTracker.Controllers;

[Authorize]
[Microsoft.AspNetCore.Components.Route("User")]
public class UserController : Controller
{
	[HttpGet]
	public async Task<IActionResult> Index()
	{
		return View("Index");
	}
}