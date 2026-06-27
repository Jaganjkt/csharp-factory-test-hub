namespace WebApplication.Services;

public class MockRadioDeviceService : ITestDeviceService
{
    private readonly Random _random = new();
    public bool IsConnected { get; private set; }
    public string? ConnectedDevice { get; private set; }

    public async Task<bool> ConnectAsync(string serialNumber)
    {
        await Task.Delay(_random.Next(500, 1500));
        if (_random.NextDouble() < 0.05)
            return false;

        IsConnected = true;
        ConnectedDevice = serialNumber;
        return true;
    }

    public async Task<TestStepResult> RunStepAsync(string stepName)
    {
        var delay = _random.Next(800, 3000);
        await Task.Delay(delay);

        var failureChance = stepName switch
        {
            "Initialize" => 0.02,
            "RF Calibration" => 0.10,
            "Power Validation" => 0.08,
            "Temperature Test" => 0.15,
            "Firmware Verification" => 0.05,
            "Network Connectivity" => 0.07,
            _ => 0.05
        };

        var passed = _random.NextDouble() > failureChance;

        var (value, unit, passMsg, failMsg) = stepName switch
        {
            "RF Calibration" => (
                passed ? -60 + _random.NextDouble() * 10 : -95 + _random.NextDouble() * 5,
                "dBm",
                "RF calibration within spec",
                "Signal strength below threshold"
            ),
            "Power Validation" => (
                passed ? 3.25 + _random.NextDouble() * 0.1 : 2.7 + _random.NextDouble() * 0.2,
                "V",
                "Voltage output nominal",
                "Voltage out of range"
            ),
            "Temperature Test" => (
                passed ? 55 + _random.NextDouble() * 20 : 88 + _random.NextDouble() * 10,
                "°C",
                "Temperature within operating range",
                "Temperature exceeded threshold"
            ),
            "Firmware Verification" => (
                (double?)null,
                (string?)null,
                "Firmware checksum verified",
                "Firmware checksum mismatch"
            ),
            "Network Connectivity" => (
                passed ? _random.NextDouble() * 0.5 : 8 + _random.NextDouble() * 5,
                "%",
                "Network connectivity test passed",
                "Packet loss rate exceeded maximum"
            ),
            "Initialize" => (
                (double?)null,
                (string?)null,
                "Device initialized successfully",
                "Device initialization failed"
            ),
            _ => (
                (double?)null,
                (string?)null,
                $"{stepName} completed",
                $"{stepName} failed"
            )
        };

        return new TestStepResult(
            stepName,
            passed,
            passed ? passMsg : failMsg,
            value,
            unit,
            delay
        );
    }

    public async Task<TelemetryData> ReadTelemetryAsync()
    {
        await Task.Delay(_random.Next(100, 300));
        return new TelemetryData(
            Temperature: 45 + _random.NextDouble() * 35,
            SignalStrength: -80 + _random.NextDouble() * 30,
            Voltage: 3.0 + _random.NextDouble() * 0.6,
            Timestamp: DateTime.Now
        );
    }

    public async Task DisconnectAsync()
    {
        await Task.Delay(_random.Next(200, 500));
        IsConnected = false;
        ConnectedDevice = null;
    }
}
