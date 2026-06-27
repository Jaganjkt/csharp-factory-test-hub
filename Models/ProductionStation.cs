namespace WebApplication.Models;

public class ProductionStation
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Status { get; set; } = "Idle";
    public string Type { get; set; } = string.Empty;
    public DateTime LastCalibrationDate { get; set; }
    public List<TestSession> Sessions { get; set; } = new();
}
