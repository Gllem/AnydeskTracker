using AnydeskTracker.Models;

namespace AnydeskTracker.DTOs;

public class AdminBotGamesDto(PcDto[] pcs)
{
	public PcDto[] Pcs { get; set; } = pcs;
}