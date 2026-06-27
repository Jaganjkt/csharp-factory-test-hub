namespace WebApplication.Models;

public class TestStep
{
    public int Id { get; set; }
    public int TestSessionId { get; set; }
    public TestSession? TestSession { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public int Order { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? ErrorMessage { get; set; }
    public double? MeasuredValue { get; set; }
    public string? Unit { get; set; }
}
