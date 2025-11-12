using System.Security.Claims;
using AnydeskTracker.Data;
using AnydeskTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

[ApiController]
[Route("api/telegram")]
public class TelegramController(ApplicationDbContext context, TelegramService telegramService) : ControllerBase
{
	private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
	
	[HttpPost("webhook")]
	public async Task<IActionResult> Webhook([FromBody] Update update)
	{
		await telegramService.HandleUpdate(update);
		return Ok();
	}

	[HttpGet("poll")]
	public async Task<IActionResult> Poll()
	{
		var updates = await telegramService.Client.GetUpdates();
		foreach (var update in updates)
		{
			await telegramService.HandleUpdate(update);
		}

		if (updates.Length > 0)
		{
			var lastId = updates.Last().Id + 1;
			await telegramService.Client.GetUpdates(offset: lastId);
		}

		return Ok(new { Count = updates.Length });
	}

	[HttpPost("test")]
	[Authorize]
	public async Task<IActionResult> TestBot()
	{
		await telegramService.SendMessageAsync(UserId, "Тест подключения бота");
		return Ok();
	}
}