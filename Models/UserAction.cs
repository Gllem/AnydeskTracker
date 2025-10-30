using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace AnydeskTracker.Models;

public class UserAction
{
	[Key]
	public int Id { get; set; }

	public string UserId { get; set; }

	[ForeignKey(nameof(UserId))]
	public IdentityUser User { get; set; }

	public int WorkSessionId { get; set; }

	[ForeignKey(nameof(WorkSessionId))]
	public WorkSessionModel WorkSession { get; set; }

	[Required]
	public string ActionType { get; set; } = string.Empty;

	public string? Description { get; set; } = string.Empty;

	public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}