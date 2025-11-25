namespace AnydeskTracker.Services;

public static class TimeSettingsService
{
	public static readonly TimeSpan PcUsageTime = TimeSpan.FromMinutes(1);
	public static readonly TimeSpan SessionTime = TimeSpan.FromMinutes(2);
	
	public static readonly TimeSpan PcCooldown = TimeSpan.FromMinutes(30);
	public static readonly TimeSpan PcForceFreeUpTime = TimeSpan.FromMinutes(1);
	
	public static readonly TimeSpan TelegramNotificationInterval = TimeSpan.FromMinutes(3);
}