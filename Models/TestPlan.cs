namespace WebApplication.Models;

public class TestPlan
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ProductType { get; set; } = string.Empty;
    public string Steps { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; } = true;
}
