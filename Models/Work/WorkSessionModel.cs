using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace AnydeskTracker.Models
{
	public class WorkSessionModel : PausableModel
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string UserId { get; set; }

		[ForeignKey(nameof(UserId))]
		public AppUser User { get; set; }
		
		public ICollection<PcUsage> ComputerUsages { get; set; } = new List<PcUsage>();
	}
}