using AnydeskTracker.Extensions;
using AnydeskTracker.Models;
using AnydeskTracker.Views.Admin;

namespace AnydeskTracker.DTOs;

public class PcDto(PcModel model)
{
	public int Id { get; set; } = model.Id;
	public string BotId { get; set; } = model.BotId;
	public int SortOrder { get; set; } = model.SortOrder;

	public string AnyDeskId { get; set; } = model.AnyDeskId;
	public string RustDeskId { get; set; } = model.RustDeskId;

	public string Password { get; set; } = model.Password;

	public PcStatus Status { get; set; } = model.Status;

	public string StatusText => Status.GetDisplayName();
	
	public DateTime LastStatusChange { get; set; } = model.LastStatusChange.ToUtc();
	public string DisplayId => model.DisplayId;
	public bool HasOverridenGames => model.OverridenBotGames.Count != 0;
	public bool AgentReady => model.AgentReady;

	public PcBotSchedule PcBotSchedule { get; set; } = model.PcBotSchedule;
	public PcBotSchedule? LastBotSchedule { get; set; } = model.LastActiveSchedule;
	public DateTime? LastLaunchTime { get; set; } = model.LastLaunchTime;
	public BotScheduleStatus? LastStatus { get; set; } = model.LastStatus;
	public string LastStatusText => LastStatus?.GetDisplayName() ?? "-";


	public DateTime? NextLaunchTime { get; set; } = model.NextLaunchTime;
}