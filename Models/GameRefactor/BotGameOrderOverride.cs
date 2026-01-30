using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnydeskTracker.Models.GameRefactor;

public class BotGameOrderOverride
{
	[Required]
	public int PcId { get; set; }

	[ForeignKey(nameof(PcId))]
	public PcModel Pc { get; set; } = null!;

	[Required]
	public int GameId { get; set; }
	[ForeignKey(nameof(GameId))]
	public Game Game { get; set; } = null!;

	[Required]
	public int Order { get; set; }
}