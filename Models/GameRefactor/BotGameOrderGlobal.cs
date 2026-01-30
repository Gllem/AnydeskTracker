using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnydeskTracker.Models.GameRefactor;

public class BotGameOrderGlobal
{
	[Key]
	public int GameId { get; set; }

	[ForeignKey(nameof(GameId))]
	public Game Game { get; set; } = null!;

	[Required]
	public int Order { get; set; }
}