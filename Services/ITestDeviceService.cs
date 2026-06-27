namespace WebApplication.Services;

public record TelemetryData(double Temperature, double SignalStrength, double Voltage, DateTime Timestamp);
public record TestStepResult(string StepName, bool Passed, string Message, double? MeasuredValue, string? Unit, int DurationMs);

public interface ITestDeviceService
{
    Task<bool> ConnectAsync(string serialNumber);
    Task<TestStepResult> RunStepAsync(string stepName);
    Task<TelemetryData> ReadTelemetryAsync();
    Task DisconnectAsync();
    bool IsConnected { get; }
    string? ConnectedDevice { get; }
}
