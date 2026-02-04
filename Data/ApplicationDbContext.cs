using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AnydeskTracker.Models;
using AnydeskTracker.Models.Game;

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
        
        BuildBlockedAgents(builder);

        builder.Entity<PcModel>(pcModel =>
        {
            pcModel.Property(x => x.AgentReady).HasDefaultValue(true);
            pcModel.OwnsOne(x => x.PcBotSchedule);
        });
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
            .WithMany(x => x.GameScheduleLinks)
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void BuildBotGameOrder(ModelBuilder builder)
    {
        builder.Entity<BotGameAssignmentGlobal>()
            .HasOne(x => x.Game)
            .WithOne(x => x.GlobalOrder)
            .HasForeignKey<BotGameAssignmentGlobal>(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<BotGameAssignmentOverride>(e =>
        {
            e.HasKey(x => new { x.PcId, x.GameId});

            e.HasOne(x => x.Game)
                .WithMany(x => x.PcOverrides)
                .HasForeignKey(x => x.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Pc)
                .WithMany(x => x.OverridenBotGames)
                .HasForeignKey(x => x.PcId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            e.Ignore("PcModelId");
        });
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
    
    public DbSet<Game> GameCatalog { get; set; }
    public DbSet<GameSchedule> GameSchedules { get; set; }
    public DbSet<BotGameAssignmentGlobal> BotGameAssignmentsGlobal  { get; set; }
    public DbSet<BotGameAssignmentOverride> BotGameAssignmentsOverride  { get; set; }

    public DbSet<UserAction> UserActions { get; set; }
    public DbSet<PcBotAction> BotActions { get; set; }
    public DbSet<PcBotDolphinAction> DolphinActions { get; set; }
    public DbSet<BlockedAgentPhone> BlockedPhoneNumbers => Set<BlockedAgentPhone>();
    public DbSet<BlockedAgentEmail> BlockedEmails => Set<BlockedAgentEmail>();
}
