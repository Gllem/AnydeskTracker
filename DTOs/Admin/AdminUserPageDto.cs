using AnydeskTracker.Data;
using AnydeskTracker.Models;
using Microsoft.AspNetCore.Identity;

namespace AnydeskTracker.DTOs;

public class AdminUserPageDto
{
	public string UserId { get; set; }
	public string? UserEmail { get; set; }
	public string UserName { get; set; }

	public WorkSessionModel[] WorkSessions;

	public AdminUserPageDto(AppUser user, WorkSessionModel[] workSessions)
	{
		UserId = user.Id;
		UserEmail = user.Email;
		UserName = user.UserName ?? String.Empty;
		WorkSessions = workSessions;
	}
}