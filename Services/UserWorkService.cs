using AnydeskTracker.Data;
using AnydeskTracker.DTOs;
using AnydeskTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace AnydeskTracker.Services
{
    public class UserWorkService(
        UserActionService actionService,
        TelegramService telegramService,
        PcService pcService,
        ApplicationDbContext context)
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
            if (computer == null || computer.Status != PcStatus.Free || !computer.AgentReady)
                return null; 

            
            await pcService.ChangePcStatus(computer, PcStatus.Busy);

            var usage = new PcUsage
            {
                PcId = computerId,
                WorkSessionId = session.Id,
                StartTime = DateTime.UtcNow,
                IsActive = true
            };

            context.PcUsages.Add(usage);
            
            await context.SaveChangesAsync();
            await actionService.LogAsync(session, ActionType.PcAssign, $"{computer.DisplayId}");

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

            await pcService.ChangePcStatus(pc, PcStatus.Broken);
            
            usage.IsActive = false;
            usage.EndTime = DateTime.UtcNow;

            await context.SaveChangesAsync();
            
            await actionService.LogAsync(session, ActionType.PcReport, $"ID: {pc.Id}, BotID: {pc.BotId}, AnyDeskID: {pc.AnyDeskId}, ПРИЧИНА: {reason}");

            var user = await context.Users.FindAsync(userId);
            
            await telegramService.SendMessageToAdmin(
                $"\u26a0\ufe0f Репорт\n" +
                $"Пользователь: {user?.UserName}\n" +
                $"ID: {pc.Id}\n" +
                $"BotID: {pc.BotId}\n" +
                $"AnyDesk ID: {pc.AnyDeskId}\n" +
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

            await FreeUpPc(activeUsage);
            await context.SaveChangesAsync();
            await actionService.LogAsync(session, ActionType.PcRelease, $"{activeUsage.Pc.DisplayId}");
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
                await FreeUpPc(activeUsage);
                await actionService.LogAsync(session, ActionType.PcRelease, $"{activeUsage.Pc.DisplayId}");
            }
            
            await context.SaveChangesAsync();
            await actionService.LogAsync(session, ActionType.SessionEnd);
        }

        public async Task PauseWorkAsync(string userId)
        {
            var workSession = await GetActiveSessionAsync(userId);

            if (workSession == null)
                return;

            if (workSession.IsPaused)
                return;

            workSession.IsPaused = true;
            workSession.PauseStartTime = DateTime.UtcNow;

            var activeUsage = workSession.ComputerUsages.FirstOrDefault(u => u.IsActive);

            if (activeUsage != null)
            {
                activeUsage.IsPaused = true;
                activeUsage.PauseStartTime = DateTime.UtcNow;
            }
            
            await actionService.LogAsync(workSession, ActionType.SessionPause);
            var user = await context.Users.FindAsync(userId);
            await telegramService.SendMessageToAdmin(
                $"\u23f8\ufe0f Пауза\n" +
                $"Пользователь: {user?.UserName}\n"
            );

            await context.SaveChangesAsync();
        }

        public async Task UnpauseWorkAsync(string userId)
        {
            var workSession = await GetActiveSessionAsync(userId);

            if (workSession == null)
                return;

            if (!workSession.IsPaused)
                return;

            var pauseDuration = DateTime.UtcNow - workSession.PauseStartTime!.Value;
            
            workSession.TotalPauseTime += pauseDuration;
            workSession.IsPaused = false;
            workSession.PauseStartTime = null;

            var activeUsage = workSession.ComputerUsages.FirstOrDefault(u => u.IsActive);

            if (activeUsage is {IsPaused: true})
            {
                var usagePauseDuration = DateTime.UtcNow - activeUsage.PauseStartTime!.Value;
                activeUsage.TotalPauseTime += usagePauseDuration;
                activeUsage.IsPaused = false;
                activeUsage.PauseStartTime = null;
            }

            await actionService.LogAsync(workSession, ActionType.SessionUnpause);
            var user = await context.Users.FindAsync(userId);
            await telegramService.SendMessageToAdmin(
                $"\u25b6\ufe0f Снял с паузы\n" +
                $"Пользователь: {user?.UserName}\n"
            );
            
            await context.SaveChangesAsync();
        }

        private async Task FreeUpPc(PcUsage pcUsage)
        {
            pcUsage.IsActive = false;
            pcUsage.EndTime = DateTime.UtcNow;

            await pcService.ChangePcStatus(pcUsage.Pc, PcStatus.CoolingDown);
        }
    }
}
