using AnydeskTracker.Models.Game;

namespace AnydeskTracker.DTOs;

public class GameDto()
{
	public int Id { get; set; }
	public string Url { get; set; }
	public string Name { get; set; }

	public GameDto(Game game) : this()
	{
		Id = game.Id;
		Url = game.Url;
		Name = game.Name;
	}

}