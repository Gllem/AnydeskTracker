using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnydeskTracker.Models.Game;

public class BotGameAssignmentGlobal
{
	[Key]
	public int GameId { get; set; }

	[ForeignKey(nameof(GameId))]
	public Game Game { get; set; }

	[Required]
	public int Order { get; set; }
}