using AnydeskTracker.Models;

namespace AnydeskTracker.DTOs;

public class AdminBotPageDto(PcModel pcModel, DateTime? lastDolphinCheck, AdminBotPageDto.BotLog[] botLogs)
{
	public int PcModelId { get; set; } = pcModel.Id;
	public string AnyDeskId { get; set; } = pcModel.AnyDeskId;
	public string? BotId { get; set; } = pcModel.BotId;
	public DateTime? LastDolphinCheck { get; set; } = lastDolphinCheck;
	public BotLog[] BotLogs { get; set; } = botLogs;

	public class BotLog(DateTime date, int dolphinChecksCount)
	{
		public string LogDate { get; set; } = $"{date.Day}.{date.Month}.{date.Year}";
		public int DolphinChecks = dolphinChecksCount;
	}
}