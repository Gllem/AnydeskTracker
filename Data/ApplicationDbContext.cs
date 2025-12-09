using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AnydeskTracker.Models;

namespace AnydeskTracker.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        

        builder.Entity<GameUserSchedule>()
            .HasOne(s => s.Game)
            .WithMany(g => g.Schedules)
            .HasForeignKey(s => s.GameId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Entity<GameUserSchedule>()
            .HasMany(s => s.Users)
            .WithMany(u => u.AssignedSchedules)
            .UsingEntity(j => j.ToTable("GameUserSchedulesUsers"));
    }

    public DbSet<PcModel?> Pcs { get; set; }
    public DbSet<WorkSessionModel> WorkSessionModels { get; set; }
    public DbSet<PcUsage> PcUsages { get; set; }
    public DbSet<GameModel> Games { get; set; }
    public DbSet<UserAction> UserActions { get; set; }
    public DbSet<PcBotAction> BotActions { get; set; }
}
