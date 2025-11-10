using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace AnydeskTracker.Models;

public class GameModel
{
	[Key]
	public int Id { get; set; }

	[Required]
	public string GameUrl { get; set; } = string.Empty;

	[Required]
	public string GameName { get; set; } = string.Empty;
	
	public ICollection<GameUserSchedule> Schedules { get; set; } = new List<GameUserSchedule>();
}