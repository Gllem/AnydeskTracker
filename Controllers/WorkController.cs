using AnydeskTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AnydeskTracker.Data;
using AnydeskTracker.DTOs;
using AnydeskTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace AnydeskTracker.Controllers
{
	[Authorize]
	[Route("Work")]
	public class WorkController(
		ApplicationDbContext context, 
		UserWorkService workService, 
		PcService pcService) : Controller
	{
		public static readonly TimeSpan pcUsageTime = TimeSpan.FromMinutes(1);
		public static readonly TimeSpan sessionTime = TimeSpan.FromMinutes(5);
		
		private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var session = await workService.GetActiveSessionAsync(UserId);
			if (session == null)
				return View("StartWork");

			var activeUsage = session.ComputerUsages.FirstOrDefault(u => u.IsActive);
			if (activeUsage != null)
				return View("ActiveComputer", new WorkSessionDto(session, activeUsage, sessionTime, PcStatusUpdater.PcCooldown));

			return View("ComputerList");
		}

		[HttpPost("Start")]
		public async Task<IActionResult> StartWork()
		{
			await workService.StartWorkAsync(UserId);
			return RedirectToAction(nameof(Index));
		}

		[HttpPost("Assign/{computerId}")]
		public async Task<IActionResult> AssignComputer(int computerId)
		{
			await workService.AssignComputerAsync(UserId, computerId);
			return RedirectToAction(nameof(Index));
		}

		[HttpPost("End")]
		public async Task<IActionResult> EndWork()
		{
			await workService.EndWorkAsync(UserId);
			return RedirectToAction(nameof(Index));
		}

		[HttpGet("FreeComputers")]
		public async Task<IActionResult> FetchFreeComputers()
		{
			var pcs = await pcService.GetAllPcs();
			return Ok(pcs.Where(x => x.Status == PcStatus.Free));
		}
		
		[HttpGet("Games")]
		public async Task<IActionResult> FetchGames()
		{
			var games = await context.Games.ToListAsync();
			return Ok(games);
		}

		[HttpGet("Active")]
		public async Task<IActionResult> GetActivePc()
		{
			var pcUsage = await workService.GetActivePcUsage(UserId);

			if (pcUsage == null)
				return Ok(null);
			
			return Ok(new PcDto(pcUsage.Pc));
		}

		[HttpGet("CheckSessionStatus")]
		public async Task<IActionResult> CheckSessionStatus()
		{
			var session = await workService.GetActiveSessionAsync(UserId);

			if (session == null)
				return BadRequest();

			bool canEndShift = DateTime.UtcNow - session.StartTime > sessionTime;

			var pcUsage = session.ComputerUsages.FirstOrDefault(x => x.IsActive);
			
			if (pcUsage == null)
				return Ok(new
				{
					CanEndShift = canEndShift,
					ShouldChangePc = false,
					ForceExit = true
				});
			
			return Ok(new
			{
				CanEndShift = canEndShift,
				ShouldChangePc = pcUsage.Pc.Status != PcStatus.Busy || DateTime.UtcNow - pcUsage.Pc.LastStatusChange > pcUsageTime,
				ForceExit = false
			});
		}

		[HttpPost("FreeUpPc")]
		public async Task<IActionResult> FreeUpPc()
		{
			bool success = await workService.FreeUpPc(UserId);

			if (!success)
				return StatusCode(StatusCodes.Status500InternalServerError);
			
			return Ok();
		}
	}
}