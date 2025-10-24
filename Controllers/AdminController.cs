using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnydeskTracker.Data;
using AnydeskTracker.DTOs;
using AnydeskTracker.Models;
using AnydeskTracker.Services;

namespace AnydeskTracker.Controllers
{
	[Authorize(Roles = "Admin")]
	[Route("api/admin")]
	[ApiController]
	public class AdminController(ApplicationDbContext context, PcService pcService) : ControllerBase
	{
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
	}
}