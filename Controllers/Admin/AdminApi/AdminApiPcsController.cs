using AnydeskTracker.Data;
using AnydeskTracker.DTOs;
using AnydeskTracker.Models;
using AnydeskTracker.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnydeskTracker.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/admin/pcs")]
[ApiController]
public class AdminApiPcsController(ApplicationDbContext dbContext, PcService pcService, SheetsService sheetsService) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetAllPcs()
	{
		var pcs = await pcService.GetAllPcs();
		return Ok(pcs);
	}

	
	[HttpPost]
	public async Task<IActionResult> AddPc([FromBody] PcModel? pc)
	{
		pc.LastStatusChange = DateTime.UtcNow;
		dbContext.Pcs.Add(pc);
		await dbContext.SaveChangesAsync();
		return Ok(pc);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> UpdatePc(int id, [FromBody] PcModel pc)
	{
		var existing = await dbContext.Pcs.FindAsync(id);
		if (existing == null) return NotFound();

		existing.AnyDeskId = pc.AnyDeskId;
		existing.Password = pc.Password;
		existing.Status = pc.Status;
		existing.LastStatusChange = DateTime.UtcNow;

		await dbContext.SaveChangesAsync();
		return Ok(existing);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeletePc(int id)
	{
		var existing = await dbContext.Pcs.FindAsync(id);
		if (existing == null) return NotFound();

		dbContext.Pcs.Remove(existing);
		await dbContext.SaveChangesAsync();
		return NoContent();
	}

	[HttpPut("bulk-update")]
	public async Task<IActionResult> BulkUpdatePcs([FromBody] PcBulkUpdateDto[] updateDtos)
	{
		foreach (var updateDto in updateDtos)
		{
			var existing = await dbContext.Pcs.FindAsync(updateDto.Id);
			if (existing == null)
				return NotFound();

			existing.AnyDeskId = updateDto.AnyDeskId;
			existing.BotId = updateDto.BotId;
			existing.Password = updateDto.Password;
			existing.SortOrder = updateDto.SortOrder;
			existing.AgentReady = updateDto.AgentReady;
			existing.RustDeskId = updateDto.RustDeskId;
		}

		await dbContext.SaveChangesAsync();
		return Ok();
	}

	
	[HttpPut("{id}/forceFreeUp")]
	public async Task<IActionResult> ForceFreeUpPc(int id)
	{
		var pc = await dbContext.Pcs.FindAsync(id);
			
		if (pc == null)
			return NotFound();
			
		if (pc.Status == PcStatus.Free)
			return BadRequest();
			
		await pcService.ChangePcStatus(pc, PcStatus.Free);
		var usage = await dbContext.PcUsages.FirstOrDefaultAsync(x => x.IsActive && x.PcId == id);

		if (usage == null)
		{
			await dbContext.SaveChangesAsync();
			return Ok("No Usage");
		}
			
		usage.IsActive = false;
		usage.EndTime = DateTime.UtcNow;
			
		await dbContext.SaveChangesAsync();
		return Ok();
	}

	[HttpPost("updatePcs")]
	public async Task<IActionResult> UpdatePcsFromSheet([FromBody] AdminGoogleSheetDto sheetDto)
	{
		var range = $"{sheetDto.SheetName}!A1:B";
		var request = sheetsService.Spreadsheets.Values.Get(sheetDto.SheetId, range);

		try
		{
			ValueRange response = await request.ExecuteAsync();

			if (response == null || response.Values == null)
				return StatusCode(StatusCodes.Status500InternalServerError);

			var table = response.Values;

			for (int i = 1; i < table.Count; i++)
			{
				var row = table[i];
				var rowLength = row.Count;
					
				if(rowLength <= 1 || row[0] == null || string.IsNullOrWhiteSpace(row[0].ToString()))
					continue;

				string anydeskPcId = row[0].ToString() ?? "";
				string password = row[1].ToString() ?? "";

				var pc = await dbContext.Pcs.FirstOrDefaultAsync(x => x != null && x.AnyDeskId == anydeskPcId);

				if(pc == null)
					continue;
					
				pc.Password = password;
			}
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

		await dbContext.SaveChangesAsync();
		return Ok();
	}
}