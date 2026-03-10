using System.ComponentModel.DataAnnotations;

namespace AnydeskTracker.Models;

public class MetrikaCollectionSettings
{
	[Key]
	public int Id { get; set; }

	public decimal RevenueThreshold { get; set; }
}