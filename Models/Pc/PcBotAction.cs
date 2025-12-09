using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AnydeskTracker.Extensions;

namespace AnydeskTracker.Models;

public class PcBotAction
{
	[Key]
	public int Id { get; set; }

	public int PcId { get; set; }

	[ForeignKey(nameof(PcId))]
	public PcModel PcModel { get; set; }

	public bool Error { get; set; }

	[Required]
	public string ProcessesStatus { get; set; } = string.Empty;
	[Required]
	public string SchedulerStatus { get; set; } = string.Empty;
	[Required]
	public string DiskStatus { get; set; } = string.Empty;
	[Required]
	public string UserStatus { get; set; } = string.Empty;
	[Required]
	public string RamStatus { get; set; } = string.Empty;

	public DateTime Timestamp { get; set; } = DateTime.UtcNow;

	[JsonIgnore] 
	public string TelegramNotificationBotStatus =>
		$"Выключенные процессы: {ProcessesStatus}\n" +
		$"Выключенные задачи планировщика: {SchedulerStatus}\n" +
		$"Диск: {(string.IsNullOrEmpty(DiskStatus) ? "OK" : DiskStatus)}\n" +
		$"Пользователь: {(string.IsNullOrEmpty(UserStatus) ? "OK" : UserStatus)}\n" +
		$"Память: {(string.IsNullOrEmpty(RamStatus) ? "OK" : RamStatus)}";
}