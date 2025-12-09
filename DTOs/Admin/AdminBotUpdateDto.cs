using System.ComponentModel.DataAnnotations;

namespace AnydeskTracker.DTOs;

public class AdminBotUpdateDto
{
	[Required]
	public string BotId { get; set; } = "";
}
	