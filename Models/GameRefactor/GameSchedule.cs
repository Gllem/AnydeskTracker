using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnydeskTracker.Models.GameRefactor;

// Привязка пользователей к игре по дням недели
public class GameSchedule
{
	[Key]
	public int Id { get; set; }
	
	[Required]
	[ForeignKey(nameof(Game))]
	public int GameId { get; set; }
	public Game Game { get; set; } = null!;

	[Required]
	public DayOfWeek DayOfWeek { get; set; }

	public ICollection<GameUserScheduleToUser> UserLinks { get; set; } = new List<GameUserScheduleToUser>();

}