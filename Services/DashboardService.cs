using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db) => _db = db;

    public async Task<DashboardStats> GetStatsAsync()
    {
        var running = await _db.TestSessions.CountAsync(s => s.Status == "Running");
        var passed = await _db.TestSessions.CountAsync(s => s.Status == "Passed");
        var failed = await _db.TestSessions.CountAsync(s => s.Status == "Failed");
        var activeStations = await _db.ProductionStations.CountAsync(s => s.Status == "Active");
        return new DashboardStats(running, passed, failed, activeStations);
    }

    public Task<List<ThroughputDataPoint>> GetThroughputDataAsync()
    {
        var data = new List<ThroughputDataPoint>();
        var rng = new Random(42);
        for (int i = 7; i >= 0; i--)
        {
            var hour = DateTime.Now.AddHours(-i).ToString("HH:00");
            data.Add(new ThroughputDataPoint(hour, rng.Next(8, 25)));
        }
        return Task.FromResult(data);
    }

    public Task<List<PassFailDataPoint>> GetPassFailDataAsync()
    {
        var data = new List<PassFailDataPoint>();
        var rng = new Random(42);
        var days = new[] { "Mon", "Tue", "Wed", "Thu", "Fri" };
        foreach (var day in days)
        {
            var passed = rng.Next(15, 30);
            var failed = rng.Next(1, 6);
            data.Add(new PassFailDataPoint(day, passed, failed));
        }
        return Task.FromResult(data);
    }

    public async Task<List<TestSession>> GetRecentSessionsAsync(int count = 10)
    {
        return await _db.TestSessions
            .Include(s => s.Device)
            .Include(s => s.Station)
            .OrderByDescending(s => s.StartTime)
            .Take(count)
            .ToListAsync();
    }
}
