using Microsoft.AspNetCore.Identity;

namespace AnydeskTracker.Models;

public class AppUser : IdentityUser
{
	public ICollection<GameUserSchedule> AssignedSchedules { get; set; } = new List<GameUserSchedule>();

	public long TelegramChatId { get; set; }

	public DateTime LastNotificationTime { get; set; }
}