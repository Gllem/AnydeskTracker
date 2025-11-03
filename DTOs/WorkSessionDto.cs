using AnydeskTracker.Models;
using Microsoft.AspNetCore.Identity;

namespace AnydeskTracker.DTOs;

public class WorkSessionDto
{
	public int Id { get; set; }
	public string UserId { get; set; }

	public IdentityUser User { get; set; }

	public DateTime StartTime { get; set; }

	public DateTime? EndTime { get; set; }
	
	public TimeSpan SessionTime { get; set; }

	public DateTime PcStartTime { get; set; }

	public TimeSpan PcUsageTime { get; set; }

	public bool IsActive { get; set; }
		
	public ICollection<PcUsage> ComputerUsages { get; set; } = new List<PcUsage>();

	public WorkSessionDto(WorkSessionModel model, PcUsage usage, TimeSpan sessionTime, TimeSpan pcUsageTime)
	{
		Id = model.Id;
		UserId = model.UserId;
		User = model.User;
		StartTime = model.StartTime;
		EndTime = model.EndTime;
		SessionTime = sessionTime;

		PcStartTime = usage.StartTime; 
		PcUsageTime = pcUsageTime;
		
		IsActive = model.IsActive;
		ComputerUsages = model.ComputerUsages;
	}
}