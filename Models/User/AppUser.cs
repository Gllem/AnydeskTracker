using AnydeskTracker.Models.Game;
using Microsoft.AspNetCore.Identity;

namespace AnydeskTracker.Models;

public class AppUser : IdentityUser
{
	public ICollection<GameUserScheduleToUser> GameScheduleLinks { get; set; } = new List<GameUserScheduleToUser>();

	public long TelegramChatId { get; set; }

	public string TelegramUserName { get; set; } = "";
	public DateTime LastNotificationTime { get; set; }
}