using AnydeskTracker.Extensions;
using AnydeskTracker.Models;

namespace AnydeskTracker.DTOs;

public class NonSensitivePcDto(PcModel model)
{
	public int Id { get; set; } = model.Id;
	public string BotId { get; set; } = model.BotId;
	public int SortOrder { get; set; } = model.SortOrder;

	public string AnyDeskId { get; set; } = model.AnyDeskId;
	public PcStatus Status { get; set; } = model.Status;

	public string StatusText => Status.GetDisplayName();
	
	public DateTime LastStatusChange { get; set; } = model.LastStatusChange.ToUtc();
	public string DisplayId => model.DisplayId;
	public bool HasOverridenGames => model.OverridenBotGames.Count != 0;
}