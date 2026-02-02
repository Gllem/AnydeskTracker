using System.ComponentModel.DataAnnotations;

namespace AnydeskTracker.Models.Game;

public class Game
{
	[Key]
	public int Id { get; set; }
	[Required]
	public string Url { get; set; } = null!;
	[Required]
	public string Name { get; set; } = null!;
	
	public ICollection<GameSchedule> UserSchedules { get; set; } = new List<GameSchedule>();
	public BotGameAssignmentGlobal? GlobalOrder { get; set; }
	public ICollection<BotGameAssignmentOverride> PcOverrides { get; set; } = new List<BotGameAssignmentOverride>();
}