using AnydeskTracker.Models;
using AnydeskTracker.Views.Admin;

namespace AnydeskTracker.DTOs;

public class BotScheduleLogDto
{
	public PcBotSchedule JobSchedule { get; set; }
	public DateTime Time { get; set; }
	public BotScheduleStatus CurrentStatus { get; set; }
	public DateTime NextLaunchTime { get; set; }
}