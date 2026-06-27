using WebApplication.Models;

namespace WebApplication.Services;

public record FaultSummary(string FailureType, int Count, string MostCommonCause, string MaxSeverity);

public interface IFaultAnalysisService
{
    Task<List<FaultRecord>> GetAllFaultsAsync();
    Task<List<FaultRecord>> GetFilteredFaultsAsync(string? severity = null, string? failureType = null);
    Task<List<FaultSummary>> GetFaultSummaryAsync();
    Task<List<TestSession>> GetFailedSessionsAsync();
}
