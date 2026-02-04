using AnydeskTracker.Data;
using AnydeskTracker.DTOs;
using AnydeskTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace AnydeskTracker.Services
{
	public class PcService(ApplicationDbContext dbContext, AgentCommandsService agentCommandsService)
	{
		public async Task<List<PcDto>> GetAllPcs()
		{
			return await dbContext.Pcs.Include(x => x.OverridenBotGames).OrderBy(x => x.SortOrder).Select(x => new PcDto(x)).ToListAsync();
		}

		public async Task<List<NonSensitivePcDto>> GetAllPcsNonSensitive()
		{
			return await dbContext.Pcs.Include(x => x.OverridenBotGames).OrderBy(x => x.SortOrder).Select(x => new NonSensitivePcDto(x)).ToListAsync();
		}

		public async Task ChangePcStatus(PcModel pcModel, PcStatus pcStatus)
		{
			pcModel.Status = pcStatus;
			pcModel.LastStatusChange = DateTime.UtcNow;

			await agentCommandsService.SendCommandToAgent(pcModel.BotId, "CheckOccupation");
		}
	}
}