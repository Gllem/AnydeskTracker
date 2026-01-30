using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnydeskTracker.Models.GameRefactor;

public class GameUserScheduleToUser
{
	[Required]
	public int GameUserScheduleId { get; set; }

	[ForeignKey(nameof(GameUserScheduleId))]
	public GameSchedule GameSchedule { get; set; } = null!;

	[Required]
	public string UserId { get; set; } = string.Empty;
	
	public AppUser User { get; set; } = null!;
}