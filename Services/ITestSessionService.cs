using WebApplication.Models;

namespace WebApplication.Services;

public interface ITestSessionService
{
    Task<List<TestSession>> GetAllSessionsAsync();
    Task<TestSession?> GetSessionByIdAsync(int id);
    Task<TestSession> CreateSessionAsync(string serialNumber, string productType, string firmwareVersion, string testProfile);
    Task UpdateSessionStatusAsync(int sessionId, string status);
    Task AddLogAsync(int sessionId, string level, string message, string? stepName = null);
    Task<List<Device>> GetDevicesAsync();
    Task<List<TestPlan>> GetTestPlansAsync();
}
