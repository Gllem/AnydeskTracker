using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnydeskTracker.Models.GameRefactor;

public class BotGameOrderOverride
{
	[Required]
	[ForeignKey(nameof(Pc))]
	public int PcId { get; set; }

	public PcModel Pc { get; set; } = null!;

	[Required]
	[ForeignKey(nameof(Game))]
	public int GameId { get; set; }

	public Game Game { get; set; } = null!;

	[Required]
	public int Order { get; set; }
}