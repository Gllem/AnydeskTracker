using System.ComponentModel.DataAnnotations;

namespace AnydeskTracker.Models;

public class BrowserRevenueModel
{
	[Key]
	public int Id { get; set; }

	public string Browser { get; set; } = null!;

	public decimal LastRevenue { get; set; }
	public decimal DeltaRevenue { get; set; }
}