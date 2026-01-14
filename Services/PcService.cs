using AnydeskTracker.Data;
using AnydeskTracker.DTOs;
using AnydeskTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace AnydeskTracker.Services
{
	public class PcService
	{
		private readonly ApplicationDbContext _dbContext;
		
		public PcService(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}
		
		public async Task<List<PcDto>> GetAllPcs()
		{
			return await _dbContext.Pcs.Include(x => x.OverrideBotGames).OrderBy(x => x.SortOrder).Select(x => new PcDto(x)).ToListAsync();
		}

		public async Task<List<NonSensitivePcDto>> GetAllPcsNonSensitive()
		{
			return await _dbContext.Pcs.OrderBy(x => x.SortOrder).Select(x => new NonSensitivePcDto(x)).ToListAsync();
		}

		public async Task<PcModel?> GetPc(int id)
		{
			return await _dbContext.Pcs.FindAsync(id);
		}
	}
}