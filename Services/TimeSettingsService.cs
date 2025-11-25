namespace AnydeskTracker.Services;

public static class TimeSettingsService
{
	/*

	USAGE TIME: 1h:40m
	SESSION TIME: 6h	

	PC COOLDOWN: 30m
	PC FORCE FREE UP TIME: 15m
	
	TELEGRAM NOTIFICATION INTERVAL: 3m

	*/ 
	
	public static readonly TimeSpan PcUsageTime = TimeSpan.FromMinutes(60 * 1 + 40);
	public static readonly TimeSpan SessionTime = TimeSpan.FromHours(6);
	
	public static readonly TimeSpan PcCooldown = TimeSpan.FromMinutes(30);
	public static readonly TimeSpan PcForceFreeUpTime = TimeSpan.FromMinutes(15);
	
	public static readonly TimeSpan TelegramNotificationInterval = TimeSpan.FromMinutes(3);
}