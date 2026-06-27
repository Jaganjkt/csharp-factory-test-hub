using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Services;

public class FaultAnalysisService : IFaultAnalysisService
{
    private readonly AppDbContext _db;

    public FaultAnalysisService(AppDbContext db) => _db = db;

    public async Task<List<FaultRecord>> GetAllFaultsAsync()
    {
        return await _db.FaultRecords
            .Include(f => f.TestSession!)
                .ThenInclude(s => s.Device)
            .OrderByDescending(f => f.OccurredAt)
            .ToListAsync();
    }

    public async Task<List<FaultRecord>> GetFilteredFaultsAsync(string? severity = null, string? failureType = null)
    {
        var query = _db.FaultRecords
            .Include(f => f.TestSession!)
                .ThenInclude(s => s.Device)
            .AsQueryable();

        if (!string.IsNullOrEmpty(severity))
            query = query.Where(f => f.Severity == severity);
        if (!string.IsNullOrEmpty(failureType))
            query = query.Where(f => f.FailureType == failureType);

        return await query.OrderByDescending(f => f.OccurredAt).ToListAsync();
    }

    public async Task<List<FaultSummary>> GetFaultSummaryAsync()
    {
        return await _db.FaultRecords
            .GroupBy(f => f.FailureType)
            .Select(g => new FaultSummary(
                g.Key,
                g.Count(),
                g.OrderByDescending(f => f.OccurredAt).First().RootCause,
                g.OrderByDescending(f => f.Severity == "Critical" ? 4 : f.Severity == "High" ? 3 : f.Severity == "Medium" ? 2 : 1).First().Severity
            ))
            .ToListAsync();
    }

    public async Task<List<TestSession>> GetFailedSessionsAsync()
    {
        return await _db.TestSessions
            .Include(s => s.Device)
            .Include(s => s.Station)
            .Include(s => s.Steps.OrderBy(st => st.Order))
            .Include(s => s.Faults)
            .Where(s => s.Status == "Failed")
            .OrderByDescending(s => s.StartTime)
            .ToListAsync();
    }
}
