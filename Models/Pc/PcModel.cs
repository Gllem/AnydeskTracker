using System;
using System.ComponentModel.DataAnnotations;

namespace AnydeskTracker.Models
{
	public enum PcStatus
	{
		[Display(Name = "Свободен")]
		Free = 0,
		[Display(Name = "Занят")]
		Busy = 1,
		[Display(Name = "Охлаждается")]
		CoolingDown = 2,
		[Display(Name = "Сломан")]
		Broken = 3
	}
	
	public class PcModel
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string PcId { get; set; } = string.Empty;

		[Required]
		public string Password { get; set; } = string.Empty;

		public string BotId { get; set; } = string.Empty;

		[Required]
		public PcStatus Status { get; set; } = PcStatus.Free;

		public DateTime LastStatusChange { get; set; } = DateTime.UtcNow;

		public int SortOrder { get; set; }
		
		public string DisplayId => 
			!string.IsNullOrEmpty(BotId) ? BotId : 
			!string.IsNullOrEmpty(PcId) ? PcId :
			SortOrder.ToString();
	}
}