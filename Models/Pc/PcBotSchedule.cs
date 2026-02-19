namespace AnydeskTracker.Models;

public class PcBotSchedule
{
	public bool Enabled { get; set; }

	public TimeSpan StartTod { get; set; }
	public int IntervalMinutes { get; set; }
	public TimeSpan? EndTod { get; set; }
}