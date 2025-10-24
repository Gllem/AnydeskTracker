using AnydeskTracker.Data;
using AnydeskTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace AnydeskTracker.Services
{
    public class UserWorkService
    {
        private readonly ApplicationDbContext _context;

        public UserWorkService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<WorkSessionModel?> GetActiveSessionAsync(string userId)
        {
            return await _context.WorkSessionModels
                .Include(s => s.ComputerUsages)
                .ThenInclude(u => u.Pc)
                .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive);
        }

        public async Task<PcUsage?> GetActivePcUsage(string userId)
        {
            var session = await GetActiveSessionAsync(userId);

            return session?.ComputerUsages.FirstOrDefault(x => x.IsActive);
        }

        public async Task<WorkSessionModel> StartWorkAsync(string userId)
        {
            var existing = await GetActiveSessionAsync(userId);
            if (existing != null)
                return existing;

            var session = new WorkSessionModel
            {
                UserId = userId,
                StartTime = DateTime.UtcNow,
                IsActive = true
            };

            _context.WorkSessionModels.Add(session);
            await _context.SaveChangesAsync();

            return session;
        }

        public async Task<PcUsage?> AssignComputerAsync(string userId, int computerId)
        {
            var session = await GetActiveSessionAsync(userId);
            if (session == null) return null;

            var computer = await _context.Pcs.FindAsync(computerId);
            if (computer == null || computer.Status != PcStatus.Free)
                return null;

            computer.Status = PcStatus.Busy;
            computer.LastStatusChange = DateTime.UtcNow;

            var usage = new PcUsage
            {
                PcId = computerId,
                WorkSessionId = session.Id,
                StartTime = DateTime.UtcNow,
                IsActive = true
            };

            _context.PcUsages.Add(usage);
            await _context.SaveChangesAsync();

            return usage;
        }

        public async Task<bool> FreeUpPc(string userId)
        {
            var session = await GetActiveSessionAsync(userId);
            if (session == null) return false;
            
            var activeUsage = session.ComputerUsages.FirstOrDefault(u => u.IsActive);
            if (activeUsage == null)
                return false;

            FreeUpPc(activeUsage);
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task EndWorkAsync(string userId)
        {
            var session = await GetActiveSessionAsync(userId);
            if (session == null) return;

            session.IsActive = false;
            session.EndTime = DateTime.UtcNow;

            var activeUsage = session.ComputerUsages.FirstOrDefault(u => u.IsActive);
            
            if (activeUsage != null) 
                FreeUpPc(activeUsage);

            await _context.SaveChangesAsync();
        }

        private void FreeUpPc(PcUsage pcUsage)
        {
            pcUsage.IsActive = false;
            pcUsage.EndTime = DateTime.UtcNow;

            pcUsage.Pc.Status = PcStatus.CoolingDown;
            pcUsage.Pc.LastStatusChange = DateTime.UtcNow;
        }
    }
}
