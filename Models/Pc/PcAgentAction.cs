using System.ComponentModel.DataAnnotations;

namespace AnydeskTracker.Models;

public class PcAgentAction
{
	[Key]
	public int Id { get; set; }

	[Required]
	public string Name { get; set; } = string.Empty;

	[Required]
	public string Description { get; set; } = string.Empty;

	[Required]
	public AgentActionType ActionType { get; set; } = AgentActionType.StatusLog;
}

public enum AgentActionType
{
	[Display(Name = "Статус")]
	StatusLog,
	[Display(Name = "Ошибка")]
	Error,
}