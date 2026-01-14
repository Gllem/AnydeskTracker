using System.ComponentModel.DataAnnotations;

namespace AnydeskTracker.Models;

public class BotGame
{
	[Key]
	public int Id { get; set; }
	[Required]
	public string GameUrl { get; set; } = string.Empty;

	public bool IsGlobal { get; set; } = true;
}