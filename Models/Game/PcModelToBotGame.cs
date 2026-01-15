namespace AnydeskTracker.Models;

public class PcModelToBotGame
{
	public int PcModelId { get; set; }
	public PcModel PcModel { get; set; }

	public int BotGameId { get; set; }
	public BotGame BotGame { get; set; }

	public int Order { get; set; }
}