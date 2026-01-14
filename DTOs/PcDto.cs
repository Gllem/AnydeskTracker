using AnydeskTracker.Extensions;
using AnydeskTracker.Models;

namespace AnydeskTracker.DTOs;

public class PcDto(PcModel model)
{
	public int Id { get; set; } = model.Id;
	public string BotId { get; set; } = model.BotId;
	public int SortOrder { get; set; } = model.SortOrder;

	public string AnyDeskId { get; set; } = model.PcId;

	public string Password { get; set; } = model.Password;

	public PcStatus Status { get; set; } = model.Status;

	public string StatusText => Status.GetDisplayName();
	
	public DateTime LastStatusChange { get; set; } = model.LastStatusChange.ToUtc();
	public string DisplayId => model.DisplayId;
	public bool HasOverridenGames => model.OverrideBotGames.Count != 0;
}