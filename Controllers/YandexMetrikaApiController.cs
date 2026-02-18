using AnydeskTracker.Data;
using AnydeskTracker.DTOs;
using AnydeskTracker.Services.MetrikaServices;
using Microsoft.AspNetCore.Mvc;

namespace AnydeskTracker.Controllers;
[Route("api/metrika")]
[ApiController]
public class YandexMetrikaApiController(YandexMetrikaService metrikaService, ApplicationDbContext dbContext)
    : ControllerBase
{
    public static Dictionary<string, string> Accounts => YandexMetrikaController.Accounts;

    [HttpGet("checkGameMetrika/{gameMetrikaId}")]
    public async Task<IActionResult> CheckGameMetrika(string gameMetrikaId)
    {
        var requestDto = new BuildRequestDto();
        var game = dbContext.GameCatalog.FirstOrDefault(x => x.YandexMetrikaId == gameMetrikaId);
        if (game == null) return NotFound();

        var accountName = game.AccountName;
        if (string.IsNullOrWhiteSpace(accountName)) return BadRequest();

        //TODO:Filters
        requestDto.Accounts.Add(Accounts.FirstOrDefault(x => x.Value == accountName).Key);
        requestDto.Period = "today";
        requestDto.EntityFields.Add("page_id");
        requestDto.Fields.Add("partner_wo_nds");

        var result = await metrikaService.BuildReportAsync(requestDto);
        if (result == null)
            return BadRequest();

        var reward = result[0].Data.Points
            .FirstOrDefault(x => x.Dimensions.Values.Any(a => a.ToString() == gameMetrikaId))
            ?.Measures.FirstOrDefault(d => d.ContainsKey("partner_wo_nds"))
            ?.GetValueOrDefault("partner_wo_nds")
            .GetDecimal();

        var previousReward = game.LastReward;
        if (reward == null) return StatusCode(500);

        game.LastReward = reward.Value;
        await dbContext.SaveChangesAsync();


        return Ok(reward - previousReward > 100);
    }
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok("works");
    }
}