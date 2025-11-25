using AnydeskTracker.Controllers;
using AnydeskTracker.Data;
using AnydeskTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace AnydeskTracker.Services;

public class TelegramNotifier(IServiceScopeFactory scopeFactory) : BackgroundService
{
	private readonly TimeSpan interval = TimeSpan.FromMinutes(1);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				using var scope = scopeFactory.CreateScope();
				var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				var tg = scope.ServiceProvider.GetRequiredService<TelegramService>();

				var now = DateTime.UtcNow;
				var sessions = 
					await db.WorkSessionModels
						.Include(x => x.User)
						.Include(x => x.ComputerUsages)
						.ThenInclude(x => x.Pc)
						.Where(x => x.IsActive && x.ComputerUsages.Count(u => u.IsActive) > 0)
						.ToListAsync(stoppingToken);
				
				foreach (var session in sessions)
				{
					var activeUsage = session.ComputerUsages.First(x => x.IsActive);
					var user = session.User;
					
					if (activeUsage.Pc.Status == PcStatus.Busy && activeUsage.TotalActiveTime > TimeSettingsService.PcUsageTime)
					{
						await NotifyUser(user, now, tg, "Необходимо сменить пк!");
					}
				}

				await db.SaveChangesAsync(stoppingToken);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ошибка: {ex}");
			}

			await Task.Delay(interval, stoppingToken);
		}
	}

	private async Task NotifyUser(AppUser user, DateTime now, TelegramService telegramService, string message)
	{
		if(user.LastNotificationTime.Add(TimeSettingsService.TelegramNotificationInterval) > now)
			return;

		await telegramService.SendMessageAsync(user.TelegramChatId, message);
		user.LastNotificationTime = now;
	}
}