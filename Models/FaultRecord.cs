namespace WebApplication.Models;

public class FaultRecord
{
    public int Id { get; set; }
    public int TestSessionId { get; set; }
    public TestSession? TestSession { get; set; }
    public string FailureType { get; set; } = string.Empty;
    public string RootCause { get; set; } = string.Empty;
    public string Severity { get; set; } = "Medium";
    public string StepName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public string? Resolution { get; set; }
}
