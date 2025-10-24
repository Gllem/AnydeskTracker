using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnydeskTracker.Models
{
	public class PcUsage
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int PcId { get; set; }

		[ForeignKey(nameof(PcId))]
		public PcModel Pc { get; set; }

		[Required]
		public int WorkSessionId { get; set; }

		[ForeignKey(nameof(WorkSessionId))]
		public WorkSessionModel WorkSession { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime? EndTime { get; set; }

		public bool IsActive { get; set; }
	}
}