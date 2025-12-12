using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnydeskTracker.Models;

public class PcBotDolphinAction
{
	[Key]
	public int Id { get; set; }

	public int PcId { get; set; }

	[ForeignKey(nameof(PcId))]
	public PcModel PcModel { get; set; }
	
	public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}