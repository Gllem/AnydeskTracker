using System.ComponentModel.DataAnnotations;

namespace AnydeskTracker.Models.Game;

public class Game
{
	[Key]
	public int Id { get; set; }
	public string? YandexMetrikaId { get; set; } = null;
	public string AccountName { get; set; } = "";
	[Required]
	public string Url { get; set; } = null!;
	[Required]
	public string Name { get; set; } = null!;
	public decimal LastReward { get; set; }
	
	public ICollection<GameSchedule> UserSchedules { get; set; } = new List<GameSchedule>();
	public BotGameAssignmentGlobal? GlobalOrder { get; set; }
	public ICollection<BotGameAssignmentOverride> PcOverrides { get; set; } = new List<BotGameAssignmentOverride>();
}