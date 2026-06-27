using WebApplication.Models;

namespace WebApplication.Services;

public record DashboardStats(int RunningTests, int PassedUnits, int FailedUnits, int ActiveStations);
public record ThroughputDataPoint(string Hour, int Count);
public record PassFailDataPoint(string Label, int Passed, int Failed);

public interface IDashboardService
{
    Task<DashboardStats> GetStatsAsync();
    Task<List<ThroughputDataPoint>> GetThroughputDataAsync();
    Task<List<PassFailDataPoint>> GetPassFailDataAsync();
    Task<List<TestSession>> GetRecentSessionsAsync(int count = 10);
}
