namespace WebApplication.Models;

public class TestSession
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public Device? Device { get; set; }
    public string TestProfile { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Operator { get; set; } = string.Empty;
    public int StationId { get; set; }
    public ProductionStation? Station { get; set; }
    public List<TestStep> Steps { get; set; } = new();
    public List<ExecutionLog> Logs { get; set; } = new();
    public List<FaultRecord> Faults { get; set; } = new();
}
