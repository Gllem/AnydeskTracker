using AnydeskTracker.Data;
using AnydeskTracker.Extensions;
using AnydeskTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace AnydeskTracker.Services;

public class BotStatusUpdater(IServiceScopeFactory scopeFactory) : BackgroundService
{
	private static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);
	private static readonly TimeSpan BotWatchdogDeathTime = TimeSpan.FromMinutes(10);
	private static readonly TimeSpan BotDolphinDeathTime = TimeSpan.FromMinutes(50);
	
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await UpdateBotStatusAsync(stoppingToken);

			await Task.Delay(Interval, stoppingToken);
		}
	}
	
	private async Task UpdateBotStatusAsync(CancellationToken stoppingToken)
	{
		try
		{
			using var scope = scopeFactory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			var telegramService = scope.ServiceProvider.GetRequiredService<TelegramService>();
			
			var now = DateTime.UtcNow;
			var computers = await db.Pcs.ToListAsync(stoppingToken);

			foreach (var pc in computers)
			{
				var lastAction = (await db.BotActions.Where(x => x.PcId == pc.Id).ToListAsync(stoppingToken)).MaxBy(x => x.Timestamp);
		
				if(lastAction == null)
					return;
				
				if(lastAction.Timestamp.Add(BotWatchdogDeathTime) > now &&
				   (pc.LastBotHttpStatusCheck == null || pc.LastBotHttpStatusCheck.Add(BotDolphinDeathTime) > now))
					return;

				await telegramService.SendMessageToAdmin(
					$"\u26a0\ufe0f Бот давно не отсылал статус!\n" +
					$"Бот: {pc.BotId}\n" +
					$"AnyDesk ID: {pc.PcId}\n\n" +
					$"Последний полученный статус: \n" +
					lastAction.TelegramNotificationBotStatus + "\n" +
					$"Время: {lastAction.Timestamp.ToUtc().ToLocalTime()}\n"+
					$"Время получения статуса от Dolphin: {pc.LastBotHttpStatusCheck.ToUtc().ToLocalTime()}\n"+
					"(По временному поясу сервера)");
			}

			await db.SaveChangesAsync(stoppingToken);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка: {ex}");
		}
	}
}