using AnydeskTracker.Data;
using AnydeskTracker.Models;
using Microsoft.AspNetCore.Identity;

namespace AnydeskTracker.DTOs;

public class AdminUserPageDto(AppUser user, WorkSessionModel[] workSessions)
{
	public string UserId { get; set; } = user.Id;
	public string? UserEmail { get; set; } = user.Email;
	public string UserName { get; set; } = user.UserName ?? String.Empty;

	public WorkSessionModel[] WorkSessions = workSessions;
}