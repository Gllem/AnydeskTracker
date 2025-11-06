using Microsoft.AspNetCore.Identity;

namespace AnydeskTracker.Models;

public class AppUser : IdentityUser
{
	public List<GameModel> AssignedGames { get; set; } = new();
}