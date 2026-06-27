using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Services;

public class TestSessionService : ITestSessionService
{
    private readonly AppDbContext _db;

    public TestSessionService(AppDbContext db) => _db = db;

    public async Task<List<TestSession>> GetAllSessionsAsync()
    {
        return await _db.TestSessions
            .Include(s => s.Device)
            .Include(s => s.Station)
            .Include(s => s.Steps.OrderBy(st => st.Order))
            .Include(s => s.Logs.OrderBy(l => l.Timestamp))
            .OrderByDescending(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<TestSession?> GetSessionByIdAsync(int id)
    {
        return await _db.TestSessions
            .Include(s => s.Device)
            .Include(s => s.Station)
            .Include(s => s.Steps.OrderBy(st => st.Order))
            .Include(s => s.Logs.OrderByDescending(l => l.Timestamp))
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<TestSession> CreateSessionAsync(string serialNumber, string productType, string firmwareVersion, string testProfile)
    {
        var device = await _db.Devices.FirstOrDefaultAsync(d => d.SerialNumber == serialNumber);
        if (device == null)
        {
            device = new Device
            {
                SerialNumber = serialNumber,
                ProductType = productType,
                FirmwareVersion = firmwareVersion,
                Status = "Testing",
                ManufactureDate = DateTime.Now
            };
            _db.Devices.Add(device);
            await _db.SaveChangesAsync();
        }
        else
        {
            device.Status = "Testing";
        }

        var station = await _db.ProductionStations.FirstOrDefaultAsync(s => s.Status == "Active" || s.Status == "Idle")
                      ?? (await _db.ProductionStations.FirstAsync());

        var session = new TestSession
        {
            DeviceId = device.Id,
            TestProfile = testProfile,
            Status = "Running",
            StartTime = DateTime.Now,
            Operator = "Current User",
            StationId = station.Id
        };

        _db.TestSessions.Add(session);
        await _db.SaveChangesAsync();

        var stepNames = new[] { "Initialize", "RF Calibration", "Power Validation", "Temperature Test", "Firmware Verification", "Network Connectivity" };
        for (int i = 0; i < stepNames.Length; i++)
        {
            _db.TestSteps.Add(new TestStep
            {
                TestSessionId = session.Id,
                Name = stepNames[i],
                Status = "Pending",
                Order = i + 1
            });
        }
        await _db.SaveChangesAsync();

        return session;
    }

    public async Task UpdateSessionStatusAsync(int sessionId, string status)
    {
        var session = await _db.TestSessions.Include(s => s.Device).FirstOrDefaultAsync(s => s.Id == sessionId);
        if (session != null)
        {
            session.Status = status;
            if (status is "Passed" or "Failed" or "Aborted")
            {
                session.EndTime = DateTime.Now;
                if (session.Device != null)
                {
                    session.Device.Status = status == "Passed" ? "Passed" : "Failed";
                    session.Device.LastTestedDate = DateTime.Now;
                }
            }
            await _db.SaveChangesAsync();
        }
    }

    public async Task AddLogAsync(int sessionId, string level, string message, string? stepName = null)
    {
        _db.ExecutionLogs.Add(new ExecutionLog
        {
            TestSessionId = sessionId,
            Timestamp = DateTime.Now,
            Level = level,
            Message = message,
            StepName = stepName
        });
        await _db.SaveChangesAsync();
    }

    public async Task<List<Device>> GetDevicesAsync() =>
        await _db.Devices.OrderBy(d => d.SerialNumber).ToListAsync();

    public async Task<List<TestPlan>> GetTestPlansAsync() =>
        await _db.TestPlans.Where(p => p.IsActive).ToListAsync();
}
