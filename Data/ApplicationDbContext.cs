using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AnydeskTracker.Models;

namespace AnydeskTracker.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<PcModel?> Pcs { get; set; }
    public DbSet<WorkSessionModel> WorkSessionModels { get; set; }
    public DbSet<PcUsage> PcUsages { get; set; }
    public DbSet<GameModel> Games { get; set; }
}
