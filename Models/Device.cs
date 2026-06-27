namespace WebApplication.Models;

public class Device
{
    public int Id { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string ProductType { get; set; } = string.Empty;
    public string FirmwareVersion { get; set; } = string.Empty;
    public string Status { get; set; } = "Idle";
    public DateTime ManufactureDate { get; set; }
    public DateTime? LastTestedDate { get; set; }
    public List<TestSession> Sessions { get; set; } = new();
}
