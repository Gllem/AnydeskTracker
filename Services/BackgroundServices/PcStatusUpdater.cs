using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnydeskTracker.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AnydeskTracker.Data;
using AnydeskTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace AnydeskTracker.Services
{
    public class PcStatusUpdater(IServiceScopeFactory scopeFactory, PcService pcService) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdatePcStatusAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task UpdatePcStatusAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var now = DateTime.UtcNow;
                var computers = await db.Pcs.ToListAsync(stoppingToken);

                foreach (var pc in computers)
                {
                    await HandlePcStatus(pc, db, now, stoppingToken);
                }

                await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex}");
            }
        }

        private async Task HandlePcStatus(PcModel? pc, ApplicationDbContext db, DateTime now, CancellationToken stoppingToken)
        {
            if(pc == null)
                return;
                        
            if (pc.Status == PcStatus.CoolingDown && pc.LastStatusChange.Add(TimeSettingsService.PcCooldown) <= now)
            {
                await pcService.ChangePcStatus(pc, PcStatus.Free);
            }

            if (pc.Status == PcStatus.Busy && pc.LastStatusChange.Add(TimeSettingsService.PcUsageTime + TimeSettingsService.PcForceFreeUpTime) <= now)
            {
                var pcUsage = 
                    await db.PcUsages.FirstOrDefaultAsync(x => x.PcId == pc.Id && x.IsActive, cancellationToken: stoppingToken);

                if(pcUsage == null || pcUsage.TotalActiveTime > TimeSettingsService.PcUsageTime + TimeSettingsService.PcForceFreeUpTime)
                {
                    await pcService.ChangePcStatus(pc, PcStatus.CoolingDown);
                    
                    FreeUpPcUsage(pcUsage);
                }
            }
        }

        private static void FreeUpPcUsage(PcUsage? usage)
        {
            if(usage == null)
                return;
                            
            usage.IsActive = false;
            usage.EndTime = DateTime.UtcNow;
        }
    }
}
