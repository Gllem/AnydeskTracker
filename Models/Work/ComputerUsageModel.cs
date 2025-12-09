using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnydeskTracker.Models
{
	public class PcUsage : PausableModel
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
		
	}
}