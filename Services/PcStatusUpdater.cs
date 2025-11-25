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
    public class PcStatusUpdater(IServiceScopeFactory scopeFactory) : BackgroundService
    {
        public static TimeSpan PcCooldown = TimeSpan.FromMinutes(30);
        public static TimeSpan PcForceFreeUpTime = TimeSpan.FromMinutes(1);

        private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(1);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
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

                await Task.Delay(CheckInterval, stoppingToken);
            }
        }

        private static async Task HandlePcStatus(PcModel? pc, ApplicationDbContext db, DateTime now, CancellationToken stoppingToken)
        {
            if(pc == null)
                return;
                        
            if (pc.Status == PcStatus.CoolingDown && pc.LastStatusChange.Add(PcCooldown) <= now)
            {
                ChangePcStatus(pc, PcStatus.Free);
            }

            if (pc.Status == PcStatus.Busy && pc.LastStatusChange.Add(WorkController.PcUsageTime + PcForceFreeUpTime) <= now)
            {
                var pcUsage = 
                    await db.PcUsages.FirstOrDefaultAsync(x => x.PcId == pc.Id && x.IsActive, cancellationToken: stoppingToken);

                if(pcUsage == null || pcUsage.TotalActiveTime > WorkController.PcUsageTime + PcForceFreeUpTime)
                {
                    ChangePcStatus(pc, PcStatus.CoolingDown);
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

        private static void ChangePcStatus(PcModel pc, PcStatus status)
        {
            pc.Status = status;
            pc.LastStatusChange = DateTime.UtcNow;
        }
    }
}
