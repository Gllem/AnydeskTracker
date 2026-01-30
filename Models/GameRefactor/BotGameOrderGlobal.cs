using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnydeskTracker.Models.GameRefactor;

public class BotGameOrderGlobal
{
	[Key]
	[ForeignKey(nameof(Game))]
	public int GameId { get; set; }

	public Game Game { get; set; } = null!;

	[Required]
	public int Order { get; set; }
}