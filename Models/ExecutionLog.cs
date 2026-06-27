namespace WebApplication.Models;

public class ExecutionLog
{
    public int Id { get; set; }
    public int TestSessionId { get; set; }
    public TestSession? TestSession { get; set; }
    public DateTime Timestamp { get; set; }
    public string Level { get; set; } = "INFO";
    public string Message { get; set; } = string.Empty;
    public string? StepName { get; set; }
}
