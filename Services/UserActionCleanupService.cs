using AnydeskTracker.Data;

namespace AnydeskTracker.Services;

public class UserActionCleanupService(IServiceProvider serviceProvider) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await CleanOldActionsAsync();
			await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
		}
	}

	private async Task CleanOldActionsAsync()
	{
		using var scope = serviceProvider.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

		var cutoff = DateTime.UtcNow.AddDays(-14);
		var oldActions = db.UserActions.Where(a => a.Timestamp < cutoff);
		var oldSessions = db.WorkSessionModels.Where(a => a.EndTime < cutoff);
		var oldUsages = db.PcUsages.Where(a => a.EndTime < cutoff);

		db.UserActions.RemoveRange(oldActions);
		db.WorkSessionModels.RemoveRange(oldSessions);
		db.PcUsages.RemoveRange(oldUsages);
		await db.SaveChangesAsync();
	}
}