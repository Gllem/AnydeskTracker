using AnydeskTracker.Models.GameRefactor;
using Microsoft.AspNetCore.Identity;

namespace AnydeskTracker.Models;

public class AppUser : IdentityUser
{
	// LEGACY
	public ICollection<GameUserSchedule> AssignedSchedules { get; set; } = new List<GameUserSchedule>();
	// LEGACY

	public ICollection<GameUserScheduleToUser> GameScheduleLinks { get; set; } = new List<GameUserScheduleToUser>();

	public long TelegramChatId { get; set; }

	public string TelegramUserName { get; set; } = "";
	public DateTime LastNotificationTime { get; set; }
}