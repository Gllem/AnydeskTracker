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
    public class PcStatusUpdater : BackgroundService
    {
        private int pcCooldownMinutes = 1;
        
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1); // проверка каждые 1 мин

        public PcStatusUpdater(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var now = DateTime.UtcNow;
                    var computers = await db.Pcs.ToListAsync(stoppingToken);

                    foreach (var pc in computers)
                    {
                        if(pc == null)
                            continue;
                        
                        if (pc.Status == PcStatus.CoolingDown && pc.LastStatusChange.AddMinutes(pcCooldownMinutes) <= now)
                        {
                            pc.Status = PcStatus.Free;
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
    }
}
