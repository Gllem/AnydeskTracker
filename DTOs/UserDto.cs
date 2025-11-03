using AnydeskTracker.Data;
using AnydeskTracker.Models;
using Microsoft.AspNetCore.Identity;

namespace AnydeskTracker.DTOs;

public class UserDto
{
	public string UserId { get; set; }
	public string UserName { get; set; }

	public WorkSessionModel[] WorkSessions;

	public UserDto(IdentityUser user, WorkSessionModel[] workSessions)
	{
		UserId = user.Id;
		UserName = user.UserName ?? String.Empty;
		WorkSessions = workSessions;
	}
}