using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnydeskTracker.Models.Game;

public class BotGameAssignmentOverride
{
	[Required]
	public int PcId { get; set; }

	public PcModel Pc { get; set; }

	[Required]
	public int GameId { get; set; }
	public Game Game { get; set; }

	[Required]
	public int Order { get; set; }
}