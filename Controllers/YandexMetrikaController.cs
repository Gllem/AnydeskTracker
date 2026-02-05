using AnydeskTracker.DTOs;
using AnydeskTracker.Services.MetrikaServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnydeskTracker.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin/Metrika")]
public class YandexMetrikaController(YandexMetrikaService metrikaService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var tree = await metrikaService.GetTreeAsync();
        return View("MetrikaIndex", tree);
    }

    [HttpPost("Build")]
    public async Task<IActionResult> Build([FromBody] BuildRequestDto request)
    {
        var result = await metrikaService.BuildReportAsync(
            request.Dimensions,
            request.Fields,request.Period);

        return Json(result); // или Ok(result)
    }
}