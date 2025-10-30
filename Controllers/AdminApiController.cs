using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnydeskTracker.Data;
using AnydeskTracker.Models;
using AnydeskTracker.Services;
using HtmlAgilityPack;

namespace AnydeskTracker.Controllers
{
	[Authorize(Roles = "Admin")]
	[Route("api/admin")]
	[ApiController]
	public class AdminApiController(HttpClient httpClient, ApplicationDbContext context, PcService pcService) : ControllerBase
	{
#region Pcs
		[HttpGet("pcs")]
		public async Task<IActionResult> GetAllPcs()
		{
			var pcs = await pcService.GetAllPcs();
			return Ok(pcs);
		}

		[HttpPost("pcs")]
		public async Task<IActionResult> AddPc([FromBody] PcModel? pc)
		{
			pc.LastStatusChange = DateTime.UtcNow;
			context.Pcs.Add(pc);
			await context.SaveChangesAsync();
			return Ok(pc);
		}

		[HttpPut("pcs/{id}")]
		public async Task<IActionResult> UpdatePc(int id, [FromBody] PcModel pc)
		{
			var existing = await context.Pcs.FindAsync(id);
			if (existing == null) return NotFound();

			existing.PcId = pc.PcId;
			existing.Password = pc.Password;
			existing.Status = pc.Status;
			existing.LastStatusChange = DateTime.UtcNow;

			await context.SaveChangesAsync();
			return Ok(existing);
		}

		[HttpDelete("pcs/{id}")]
		public async Task<IActionResult> DeletePc(int id)
		{
			var existing = await context.Pcs.FindAsync(id);
			if (existing == null) return NotFound();

			context.Pcs.Remove(existing);
			await context.SaveChangesAsync();
			return NoContent();
		}
#endregion

#region Games
		[HttpGet("games")]
		public async Task<IActionResult> GetAllGames()
		{
			var games = await context.Games.ToListAsync();
			return Ok(games);
		}

		[HttpPost("games")]
		public async Task<IActionResult> AddGame([FromBody] GameModel? game)
		{
			if (game == null)
				return BadRequest();
			
			context.Games.Add(game);
			
			await context.SaveChangesAsync();
			return Ok(game);
		}


		[HttpPut("games/{id}")]
		public async Task<IActionResult> UpdateGame(int id, [FromBody] GameModel game)
		{
			var existing = await context.Games.FindAsync(id);
			if (existing == null) return NotFound();

			existing.GameUrl = game.GameUrl;
			
			await context.SaveChangesAsync();
			return Ok(existing);
		}

		[HttpDelete("games/{id}")]
		public async Task<IActionResult> DeleteGame(int id)
		{
			var existing = await context.Games.FindAsync(id);
			if (existing == null) return NotFound();

			context.Games.Remove(existing);
			await context.SaveChangesAsync();
			return NoContent();
		}
#endregion
	}
}