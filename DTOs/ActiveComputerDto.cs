using AnydeskTracker.Controllers;
using AnydeskTracker.Models;
using AnydeskTracker.Services;

namespace AnydeskTracker.DTOs;

public class ActiveComputerDto
{
	public DateTime ApproximateSessionEndTime { get; set; }
	public DateTime ApproximatePcUsageEndTime { get; set; }

	public PcDto PcDto { get; set; }

	public bool IsPaused { get; set; }

	public TimeSpan? PausedTime { get; set; }

	public ActiveComputerDto(WorkSessionModel workSession, PcUsage pcUsage)
	{
		ApproximateSessionEndTime = workSession.StartTime + TimeSettingsService.SessionTime + 
		                            (workSession.CurrentPauseTime ?? TimeSpan.Zero);
		ApproximatePcUsageEndTime = workSession.StartTime + TimeSettingsService.PcUsageTime +
		                            (pcUsage.CurrentPauseTime ?? TimeSpan.Zero);

		PcDto = new PcDto(pcUsage.Pc);

		PausedTime = workSession.CurrentPauseTime;
		
		IsPaused = workSession.IsPaused;
	}
}