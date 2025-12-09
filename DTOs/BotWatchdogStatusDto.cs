using System.ComponentModel.DataAnnotations;

namespace AnydeskTracker.DTOs;

public class BotWatchdogStatusDto
{
	[Required]
	public string BotId { get; set; } = String.Empty;
	public bool Error { get;  set; }
	[Required]
	public Dictionary<string, bool> StatusChecks { get; set; } = new ();
	[Required]
	public Dictionary<string, string> ErrorDescriptions { get; set; } = new();
}