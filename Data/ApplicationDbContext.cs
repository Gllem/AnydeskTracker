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
        

        BuildGameUserSchedule(builder);

        BuildBlockedAgents(builder);
        
        BuildBotGames(builder);
        
        builder.Entity<PcModel>().Property(x => x.AgentReady)
            .HasDefaultValue(true);
    }

    private static void BuildBotGames(ModelBuilder builder)
    {
        builder.Entity<PcModelToBotGame>()
            .HasKey(x => new { x.PcModelId, x.BotGameId });
        
        builder.Entity<PcModelToBotGame>()
            .HasOne(x => x.PcModel)
            .WithMany(b => b.OverrideBotGames)
            .HasForeignKey(x => x.PcModelId);

        builder.Entity<PcModelToBotGame>()
            .HasOne(x => x.BotGame)
            .WithMany()
            .HasForeignKey(x => x.BotGameId);
    }

    private static void BuildGameUserSchedule(ModelBuilder builder)
    {
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

    private static void BuildBlockedAgents(ModelBuilder builder)
    {
        builder.Entity<BlockedAgentPhone>(e =>
        {
            e.ToTable("BlockedAgentPhones");
            e.HasKey(x => x.Phone);
            e.Property(x => x.Phone)
                .HasMaxLength(32)
                .IsRequired();
        });

        builder.Entity<BlockedAgentEmail>(e =>
        {
            e.ToTable("BlockedAgentEmails");
            e.HasKey(x => x.Email);
            e.Property(x => x.Email)
                .HasMaxLength(320)
                .IsRequired();
        });
    }

    public DbSet<PcModel> Pcs { get; set; }
    public DbSet<WorkSessionModel> WorkSessionModels { get; set; }
    public DbSet<PcUsage> PcUsages { get; set; }
    public DbSet<GameModel> Games { get; set; }
    public DbSet<BotGame> BotGames { get; set; }
    public DbSet<PcModelToBotGame> PcModelToBotGames { get; set; }
    public DbSet<UserAction> UserActions { get; set; }
    public DbSet<PcBotAction> BotActions { get; set; }
    public DbSet<PcAgentAction> AgentActions { get; set; }
    public DbSet<PcBotDolphinAction> DolphinActions { get; set; }
    public DbSet<BlockedAgentPhone> BlockedPhoneNumbers => Set<BlockedAgentPhone>();
    public DbSet<BlockedAgentEmail> BlockedEmails => Set<BlockedAgentEmail>();
}
