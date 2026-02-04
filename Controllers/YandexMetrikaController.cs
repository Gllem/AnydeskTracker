using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnydeskTracker.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin/Metrika")]
public class YandexMetrikaController : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View("MetrikaIndex");
    }
}