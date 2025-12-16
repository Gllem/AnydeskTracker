using System.Globalization;

namespace AnydeskTracker.Services;

public class DailyParserService(IServiceProvider serviceProvider, ILogger<DailyParserService> logger) : BackgroundService
{
	private static readonly TimeSpan RunTimeUtc = TimeSpan.FromHours(0);
	
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			using var scope = serviceProvider.CreateScope();
			var parserService = scope.ServiceProvider.GetRequiredService<ParserService>();
			
			var nextRun = GetNextRunTimeUtc(RunTimeUtc);
			var delay = nextRun - DateTime.UtcNow;

			if (delay < TimeSpan.Zero)
				delay = TimeSpan.Zero;

			logger.LogInformation("Next parse at {time}", nextRun.ToString(CultureInfo.InvariantCulture));

			await Task.Delay(delay, stoppingToken);

			try
			{
				await parserService.FetchBlockedCredentials(stoppingToken);
			}
			catch (OperationCanceledException)
			{
				// graceful shutdown
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Parsing failed");
			}
		}
	}
	
	static DateTime GetNextRunTimeUtc(TimeSpan runAtUtc)
	{
		var now = DateTime.UtcNow;
		var todayRun = now.Date.Add(runAtUtc);

		if (todayRun > now)
			return todayRun;

		return todayRun.AddDays(1);
	}
}