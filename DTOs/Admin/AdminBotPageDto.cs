using AnydeskTracker.Models;

namespace AnydeskTracker.DTOs;

public class AdminBotPageDto(PcModel pcModel)
{
	public int PcModelId { get; set; } = pcModel.Id;
	public string PcId { get; set; } = pcModel.PcId;
	public string? BotId { get; set; } = pcModel.BotId;
	public DateTime LastDolphinCheck { get; set; } = pcModel.LastBotHttpStatusCheck;
}