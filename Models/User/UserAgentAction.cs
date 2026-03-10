using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnydeskTracker.Models;

public enum UserLogType
{
	[Display(Name = "Вызов окна")]
	WindowOpen,
	[Display(Name = "Открытие браузера")]
	BrowserOpen,
	[Display(Name = "Смена игры")]
	GameSelectionUpdated,
	[Display(Name = "Проверка дохода")]
	RevenueCheck
}

public class UserAgentAction
{
	[Key]
	public int Id { get; set; }

	public string UserId { get; set; }

	[ForeignKey(nameof(UserId))]
	public AppUser User { get; set; }

	public int WorkSessionId { get; set; }

	[ForeignKey(nameof(WorkSessionId))]
	public WorkSessionModel WorkSession { get; set; }

	public int PcId { get; set; }

	[ForeignKey(nameof(PcId))]
	public PcModel PcModel { get; set; }

	public UserLogType UserLogType { get; set; }
	public string AdditionalParams { get; set; } = null!;
	
	public DateTime Timestamp { get; set; } = DateTime.UtcNow;

}