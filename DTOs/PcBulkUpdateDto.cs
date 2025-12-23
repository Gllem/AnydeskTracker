namespace AnydeskTracker.DTOs;

public class PcBulkUpdateDto
{
	public int Id { get; set; }
	public int SortOrder { get; set; }

	public string BotId { get; set; } = string.Empty;

	public string AnyDeskId { get; set; } = string.Empty;

	public string Password { get; set; } = string.Empty;
}