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
        
        builder.Entity<GameModel>()
            .HasMany(g => g.Users)
            .WithMany(u => u.AssignedGames)
            .UsingEntity(j => j.ToTable("GameUsers"));
    }

    public DbSet<PcModel?> Pcs { get; set; }
    public DbSet<WorkSessionModel> WorkSessionModels { get; set; }
    public DbSet<PcUsage> PcUsages { get; set; }
    public DbSet<GameModel> Games { get; set; }
    public DbSet<UserAction> UserActions { get; set; }
}
