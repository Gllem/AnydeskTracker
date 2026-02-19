using AnydeskTracker.Models;

namespace AnydeskTracker.DTOs;

public class AdminBotScheduleDto
{
	public PcDto PcDto { get; set; }
	public PcBotSchedule PcBotSchedule { get; set; }
}