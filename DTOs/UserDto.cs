using AnydeskTracker.Data;
using AnydeskTracker.Models;

namespace AnydeskTracker.DTOs;

public class UserDto
{
	public string UserId { get; set; }
	public string UserName { get; set; }

	public WorkSessionModel[] WorkSessions;

	public UserDto(ApplicationDbContext context, string userId)
	{
		var user = context.Users.First(x => x.Id == userId);
		UserId = userId;
		UserName = user.UserName ?? String.Empty;
		WorkSessions = context.WorkSessionModels.Where(x => x.User == user).ToArray();
	}
}