using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AnydeskTracker.Models;

public class GameUserSchedule
{
	[Key]
	public int Id { get; set; }

	[Required]
	public int GameId { get; set; }
	[ForeignKey(nameof(GameId))]
	public GameModel Game { get; set; }

	public DayOfWeek DayOfWeek { get; set; }

	public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
}