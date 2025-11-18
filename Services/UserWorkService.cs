using AnydeskTracker.Data;
using AnydeskTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace AnydeskTracker.Services
{
    public class UserWorkService(UserActionService actionService, TelegramService telegramService, ApplicationDbContext context)
    {
        public async Task<WorkSessionModel?> GetActiveSessionAsync(string userId)
        {
            return await context.WorkSessionModels
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

            context.WorkSessionModels.Add(session);
            
            await context.SaveChangesAsync();
            await actionService.LogAsync(session, ActionType.SessionStart);
            
            return session;
        }

        public async Task<PcUsage?> AssignComputerAsync(string userId, int computerId)
        {
            var session = await GetActiveSessionAsync(userId);
            if (session == null) return null;

            var computer = await context.Pcs.FindAsync(computerId);
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

            context.PcUsages.Add(usage);
            
            await context.SaveChangesAsync();
            await actionService.LogAsync(session, ActionType.PcAssign, $"{computerId}");

            return usage;
        }

        public async Task<bool> ReportPcAsync(string userId, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                return false;
            
            var session = await GetActiveSessionAsync(userId);
            if (session == null)
                return false;
            
            var usage = session.ComputerUsages.FirstOrDefault(u => u.IsActive);
            if (usage == null)
                return false;
            
            var pc = usage.Pc;
            
            pc.Status = PcStatus.Broken;
            pc.LastStatusChange = DateTime.UtcNow;
            
            usage.IsActive = false;
            usage.EndTime = DateTime.UtcNow;

            await context.SaveChangesAsync();
            
            await actionService.LogAsync(session, ActionType.PcReport, $"ID: {pc.Id}, ПРИЧИНА: {reason}");

            var user = await context.Users.FindAsync(userId);
            
            await telegramService.SendMessageToAdmin(
                $"\u26a0\ufe0f Репорт\n" +
                $"Пользователь: {user?.UserName}\n" +
                $"ID: {pc.Id}\n" +
                $"AnyDesk ID: {pc.PcId}\n" +
                $"Причина: {reason}"
                );

            return true;
        }

        public async Task<bool> FreeUpPc(string userId)
        {
            var session = await GetActiveSessionAsync(userId);
            if (session == null) return false;
            
            var activeUsage = session.ComputerUsages.FirstOrDefault(u => u.IsActive);
            if (activeUsage == null)
                return false;

            FreeUpPc(activeUsage);
            await context.SaveChangesAsync();
            await actionService.LogAsync(session, ActionType.PcRelease, $"{activeUsage.PcId}");
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
            {
                FreeUpPc(activeUsage);
                await actionService.LogAsync(session, ActionType.PcRelease, $"{activeUsage.PcId}");
            }
            
            await context.SaveChangesAsync();
            await actionService.LogAsync(session, ActionType.SessionEnd);
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
