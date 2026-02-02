using AnydeskTracker.Models;
using AnydeskTracker.Models.Game;

namespace AnydeskTracker.DTOs;

public class AdminBotGamesDto(PcDto[] pcs, GameDto[] games)
{
	public PcDto[] Pcs { get; set; } = pcs;
	public GameDto[] Games { get; set; } = games;
}