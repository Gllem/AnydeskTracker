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
	public string UserId { get; set; } = string.Empty;
	
	[ForeignKey(nameof(UserId))]
	public AppUser User { get; set; } = null!;
}