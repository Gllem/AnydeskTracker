namespace AnydeskTracker.Models;

public class PausableModel
{
	public DateTime StartTime { get; set; }
	
	public DateTime? EndTime { get; set; }

	public bool IsPaused { get; set; }
	public DateTime? PauseStartTime { get; set; }
	public TimeSpan TotalPauseTime { get; set; }
	
	public bool IsActive { get; set; }
	
	public TimeSpan? TotalActiveTime => 
		DateTime.UtcNow - StartTime // Active Time
		                - TotalPauseTime // Calculated Pause Time
		                - (IsPaused ? (DateTime.UtcNow - PauseStartTime) : TimeSpan.Zero); // Current Pause Time
}