using System.ComponentModel.DataAnnotations;

namespace AnydeskTracker.Models;

public class GameModel
{
	[Key]
	public int Id { get; set; }

	[Required]
	public string GameUrl { get; set; } = string.Empty;
}