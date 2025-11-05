using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AnydeskTracker.Extensions;
using Microsoft.AspNetCore.Identity;

namespace AnydeskTracker.Models;

public enum ActionType
{
	[Display(Name = "Начало сессии")]
	SessionStart,
	[Display(Name = "Конец сессии")]
	SessionEnd,
	[Display(Name = "Взял пк")]
	PcAssign,
	[Display(Name = "Отпустил пк")]
	PcRelease,
}

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
	public ActionType ActionType { get; set; }

	public string ActionTypeText => ActionType.GetDisplayName();
	
	public string? Description { get; set; } = string.Empty;

	public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}