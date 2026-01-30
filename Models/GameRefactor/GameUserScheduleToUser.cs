using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnydeskTracker.Models.GameRefactor;

public class GameUserScheduleToUser
{
	[Required]
	[ForeignKey(nameof(GameSchedule))]
	public int GameUserScheduleId { get; set; }

	public GameSchedule GameSchedule { get; set; } = null!;

	[Required]
	[ForeignKey(nameof(User))]
	public string UserId { get; set; } = string.Empty;

	public AppUser User { get; set; } = null!;
}