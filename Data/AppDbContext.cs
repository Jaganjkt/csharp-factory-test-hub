using Microsoft.EntityFrameworkCore;
using WebApplication.Models;

namespace WebApplication.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Device> Devices => Set<Device>();
    public DbSet<TestPlan> TestPlans => Set<TestPlan>();
    public DbSet<TestSession> TestSessions => Set<TestSession>();
    public DbSet<TestStep> TestSteps => Set<TestStep>();
    public DbSet<ExecutionLog> ExecutionLogs => Set<ExecutionLog>();
    public DbSet<FaultRecord> FaultRecords => Set<FaultRecord>();
    public DbSet<ProductionStation> ProductionStations => Set<ProductionStation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestSession>()
            .HasOne(s => s.Device)
            .WithMany(d => d.Sessions)
            .HasForeignKey(s => s.DeviceId);

        modelBuilder.Entity<TestSession>()
            .HasOne(s => s.Station)
            .WithMany(st => st.Sessions)
            .HasForeignKey(s => s.StationId);

        modelBuilder.Entity<TestStep>()
            .HasOne(ts => ts.TestSession)
            .WithMany(s => s.Steps)
            .HasForeignKey(ts => ts.TestSessionId);

        modelBuilder.Entity<ExecutionLog>()
            .HasOne(l => l.TestSession)
            .WithMany(s => s.Logs)
            .HasForeignKey(l => l.TestSessionId);

        modelBuilder.Entity<FaultRecord>()
            .HasOne(f => f.TestSession)
            .WithMany(s => s.Faults)
            .HasForeignKey(f => f.TestSessionId);
    }
}
