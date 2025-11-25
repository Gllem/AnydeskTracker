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
		public static readonly TimeSpan PcUsageTime = TimeSpan.FromMinutes(60 + 40);
		public static readonly TimeSpan SessionTime = TimeSpan.FromHours(6);
		
		private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var session = await workService.GetActiveSessionAsync(UserId);
			if (session == null)
				return View("StartWork");

			var activeUsage = session.ComputerUsages.FirstOrDefault(u => u.IsActive);
			if (activeUsage != null)
				return View("ActiveComputer", new WorkSessionDto(session, activeUsage, SessionTime, PcUsageTime));

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
		
		[HttpPost("ReportPc")]
		public async Task<IActionResult> ReportPc([FromBody] string reportReason)
		{
			bool res = await workService.ReportPcAsync(UserId, reportReason);
			
			if (!res)
				return BadRequest();
			
			return Ok();
		}

		[HttpPost("End")]
		public async Task<IActionResult> EndWork()
		{
			await workService.EndWorkAsync(UserId);
			return RedirectToAction(nameof(Index));
		}

		[HttpPost("Pause")]
		public async Task<IActionResult> Pause()
		{
			await workService.PauseWorkAsync(UserId);
			return RedirectToAction(nameof(Index));
		}

		[HttpPost("Unpause")]
		public async Task<IActionResult> Unpause()
		{
			await workService.UnpauseWorkAsync(UserId);
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
			var user = await context.Users.Include(u => u.AssignedSchedules).FirstOrDefaultAsync(x => x.Id == UserId);
			
			if (user == null)
				return NotFound();

			var schedule =
				user.AssignedSchedules
					.Where(x => x.DayOfWeek == DateTime.UtcNow.ToLocalTime().DayOfWeek)
					.ToList();

			var gameIds = schedule.Select(x => x.GameId);
			
			var games = context.Games.Where(x => gameIds.Contains(x.Id));
			
			return Ok(games.Select(x => new
			{
				x.Id,
				x.GameName,
				x.GameUrl
			}));
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

			bool canEndShift = session.TotalActiveTime > SessionTime;

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
				ShouldChangePc = pcUsage.Pc.Status != PcStatus.Busy || DateTime.UtcNow - pcUsage.Pc.LastStatusChange > PcUsageTime,
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