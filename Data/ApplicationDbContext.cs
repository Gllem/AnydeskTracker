using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AnydeskTracker.Models;
using AnydeskTracker.Models.GameRefactor;

namespace AnydeskTracker.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<AppUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<Game>()
            .HasIndex(x => x.Url)
            .IsUnique();
        
        BuildGameUserSchedule(builder);
        BuildBotGameOrder(builder);
        
        BuildLegacyGameUserSchedule(builder);
        BuildLegacyBotGames(builder);
        
        BuildBlockedAgents(builder);
        
        builder.Entity<PcModel>().Property(x => x.AgentReady)
            .HasDefaultValue(true);
    }

    private static void BuildGameUserSchedule(ModelBuilder builder)
    {
        builder.Entity<GameUserScheduleToUser>()
            .HasKey(x => new {x.GameUserScheduleId, x.UserId});

        builder.Entity<GameUserScheduleToUser>()
            .HasOne(x => x.GameSchedule)
            .WithMany(x => x.UserLinks)
            .HasForeignKey(x => x.GameUserScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GameUserScheduleToUser>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void BuildBotGameOrder(ModelBuilder builder)
    {
        builder.Entity<BotGameOrderGlobal>()
            .HasOne(x => x.Game)
            .WithOne(x => x.GlobalOrder)
            .HasForeignKey<BotGameOrderGlobal>(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<BotGameOrderOverride>()
            .HasKey(x => new {x.PcId, x.GameId});

        builder.Entity<BotGameOrderOverride>()
            .HasOne(x => x.Game)
            .WithMany(x => x.PcOverrides)
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<BotGameOrderOverride>()
            .HasOne(x => x.Pc)
            .WithMany()
            .HasForeignKey(x => x.PcId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    
    private static void BuildLegacyGameUserSchedule(ModelBuilder builder)
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
    
    private static void BuildLegacyBotGames(ModelBuilder builder)
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

    
    // OLD GAMES
    public DbSet<GameModel> Games { get; set; }
    public DbSet<BotGame> BotGames { get; set; }
    public DbSet<PcModelToBotGame> PcModelToBotGames { get; set; }
    //
    
    // NEW GAMES

    public DbSet<Game> GameCatalog { get; set; }

    //
    
    public DbSet<UserAction> UserActions { get; set; }
    public DbSet<PcBotAction> BotActions { get; set; }
    public DbSet<PcBotDolphinAction> DolphinActions { get; set; }
    public DbSet<BlockedAgentPhone> BlockedPhoneNumbers => Set<BlockedAgentPhone>();
    public DbSet<BlockedAgentEmail> BlockedEmails => Set<BlockedAgentEmail>();
}
