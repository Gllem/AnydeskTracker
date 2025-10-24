using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace AnydeskTracker.Models
{
	public class WorkSessionModel
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string UserId { get; set; }

		[ForeignKey(nameof(UserId))]
		public IdentityUser User { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime? EndTime { get; set; }

		public bool IsActive { get; set; }
		
		public ICollection<PcUsage> ComputerUsages { get; set; } = new List<PcUsage>();
	}
}