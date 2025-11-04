using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        public static TimeSpan PcCooldown = TimeSpan.FromMinutes(1);
        public static TimeSpan PcForceFreeUpTime = TimeSpan.FromMinutes(15);

        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1); // проверка каждые 1 мин

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
                        if(pc == null)
                            continue;
                        
                        if (pc.Status == PcStatus.CoolingDown && pc.LastStatusChange.Add(PcCooldown) <= now)
                        {
                            ChangePcStatus(pc, PcStatus.Free);
                        }

                        if (pc.Status == PcStatus.Busy && pc.LastStatusChange.Add(PcForceFreeUpTime) <= now)
                        {
                            ChangePcStatus(pc, PcStatus.CoolingDown);

                            var usage = await db.PcUsages.FirstOrDefaultAsync(x => x.PcId == pc.Id && x.IsActive, cancellationToken: stoppingToken);
                            
                            FreeUpPcUsage(usage);
                        }
                    }

                    await db.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex}");
                }

                await Task.Delay(_interval, stoppingToken);
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
