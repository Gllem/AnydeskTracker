using System.ComponentModel.DataAnnotations;

namespace AnydeskTracker.Models.GameRefactor;

public class Game
{
	[Key]
	public int Id { get; set; }
	[Required]
	public string Url { get; set; } = null!;
	[Required]
	public string Name { get; set; } = null!;
	
	public ICollection<GameSchedule> UserSchedules { get; set; } = new List<GameSchedule>();
	public BotGameOrderGlobal? GlobalOrder { get; set; }
	public ICollection<BotGameOrderOverride> PcOverrides { get; set; } = new List<BotGameOrderOverride>();
}