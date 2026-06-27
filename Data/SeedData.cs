using WebApplication.Models;

namespace WebApplication.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Devices.Any()) return;

        var stations = new List<ProductionStation>();
        var stationTypes = new[] { "RF Test", "Power Test", "Thermal Test", "Integration Test", "Final QA" };
        var locations = new[] { "Line A - Bay 1", "Line A - Bay 2", "Line B - Bay 1", "Line B - Bay 2", "Line C - Bay 1" };
        var stationStatuses = new[] { "Active", "Active", "Active", "Idle", "Maintenance" };

        for (int i = 0; i < 5; i++)
        {
            stations.Add(new ProductionStation
            {
                Name = $"Station-{i + 1:D2}",
                Location = locations[i],
                Status = stationStatuses[i],
                Type = stationTypes[i],
                LastCalibrationDate = DateTime.Now.AddDays(-Random.Shared.Next(1, 90))
            });
        }
        context.ProductionStations.AddRange(stations);
        context.SaveChanges();

        var productTypes = new[] { "RBS 6601", "Radio 4478", "Baseband 6630", "Router 6274", "Antenna AIR 3246" };
        var firmwareVersions = new[] { "v3.2.1", "v4.0.0", "v3.8.5", "v2.1.0", "v5.0.2" };
        var devices = new List<Device>();

        for (int i = 0; i < 30; i++)
        {
            var typeIdx = i % productTypes.Length;
            devices.Add(new Device
            {
                SerialNumber = $"ERC-{2024000 + i:D7}",
                ProductType = productTypes[typeIdx],
                FirmwareVersion = firmwareVersions[typeIdx],
                Status = i < 3 ? "Testing" : (i < 20 ? "Passed" : (i < 25 ? "Failed" : "Idle")),
                ManufactureDate = DateTime.Now.AddDays(-Random.Shared.Next(30, 365)),
                LastTestedDate = i < 25 ? DateTime.Now.AddHours(-Random.Shared.Next(1, 72)) : null
            });
        }
        context.Devices.AddRange(devices);
        context.SaveChanges();

        var testProfiles = new[] { "Full Production Test", "RF Calibration Only", "Thermal Stress Test", "Quick Validation", "Firmware Verification" };
        var operators = new[] { "E. Lindqvist", "A. Johansson", "M. Andersson", "K. Svensson", "L. Eriksson" };
        var stepNames = new[] { "Initialize", "RF Calibration", "Power Validation", "Temperature Test", "Firmware Verification", "Network Connectivity" };

        var sessions = new List<TestSession>();
        for (int i = 0; i < 10; i++)
        {
            var isFailed = i >= 5 && i < 10 && (i % 2 == 0);
            var isRunning = i < 2;
            var status = isRunning ? "Running" : (isFailed ? "Failed" : "Passed");
            var startTime = DateTime.Now.AddHours(-Random.Shared.Next(1, 48));

            var session = new TestSession
            {
                DeviceId = devices[i].Id,
                TestProfile = testProfiles[i % testProfiles.Length],
                Status = status,
                StartTime = startTime,
                EndTime = isRunning ? null : startTime.AddMinutes(Random.Shared.Next(5, 45)),
                Operator = operators[i % operators.Length],
                StationId = stations[i % stations.Count].Id
            };
            sessions.Add(session);
        }
        context.TestSessions.AddRange(sessions);
        context.SaveChanges();

        foreach (var session in sessions)
        {
            var failAtStep = session.Status == "Failed" ? Random.Shared.Next(2, stepNames.Length) : -1;

            for (int j = 0; j < stepNames.Length; j++)
            {
                string stepStatus;
                string? errorMsg = null;
                double? measuredValue = null;
                string? unit = null;

                if (session.Status == "Running" && j >= 3)
                {
                    stepStatus = j == 3 ? "Running" : "Pending";
                }
                else if (j == failAtStep)
                {
                    stepStatus = "Failed";
                    errorMsg = stepNames[j] switch
                    {
                        "RF Calibration" => "Signal strength below threshold: -95 dBm (min: -85 dBm)",
                        "Power Validation" => "Voltage out of range: 2.8V (expected: 3.3V ± 0.1V)",
                        "Temperature Test" => "Temperature exceeded threshold: 92°C (max: 85°C)",
                        "Firmware Verification" => "Checksum mismatch: expected 0xA3F2, got 0xB1D4",
                        "Network Connectivity" => "Packet loss rate: 12% (max: 1%)",
                        _ => "Unexpected failure during test execution"
                    };
                }
                else if (j > failAtStep && failAtStep >= 0)
                {
                    stepStatus = "Skipped";
                }
                else
                {
                    stepStatus = "Passed";
                    measuredValue = stepNames[j] switch
                    {
                        "RF Calibration" => -65 + Random.Shared.NextDouble() * 10,
                        "Power Validation" => 3.25 + Random.Shared.NextDouble() * 0.1,
                        "Temperature Test" => 55 + Random.Shared.NextDouble() * 20,
                        _ => null
                    };
                    unit = stepNames[j] switch
                    {
                        "RF Calibration" => "dBm",
                        "Power Validation" => "V",
                        "Temperature Test" => "°C",
                        _ => null
                    };
                }

                context.TestSteps.Add(new TestStep
                {
                    TestSessionId = session.Id,
                    Name = stepNames[j],
                    Status = stepStatus,
                    Order = j + 1,
                    StartTime = session.StartTime.AddMinutes(j * 3),
                    EndTime = stepStatus is "Pending" or "Running" ? null : session.StartTime.AddMinutes(j * 3 + 2),
                    ErrorMessage = errorMsg,
                    MeasuredValue = measuredValue,
                    Unit = unit
                });
            }

            var logEntries = new[]
            {
                (0, "INFO", "Test session initialized", "Initialize"),
                (1, "INFO", $"Device {session.Device?.SerialNumber ?? "unknown"} connected", "Initialize"),
                (2, session.Status == "Failed" ? "ERROR" : "PASS", session.Status == "Failed" ? "Test step failed - see error details" : "All test steps completed successfully", stepNames[Math.Min(3, stepNames.Length - 1)])
            };

            foreach (var (offset, level, msg, step) in logEntries)
            {
                context.ExecutionLogs.Add(new ExecutionLog
                {
                    TestSessionId = session.Id,
                    Timestamp = session.StartTime.AddMinutes(offset),
                    Level = level,
                    Message = msg,
                    StepName = step
                });
            }
        }
        context.SaveChanges();

        var faultTypes = new[]
        {
            ("Temperature", "Thermal overload due to insufficient heatsink contact", "High", "Temperature Test", "Heatsink thermal paste degraded, causing poor thermal conductivity"),
            ("RF Signal", "Low signal output power below specification", "Critical", "RF Calibration", "PA module degradation detected, component replacement required"),
            ("Power Supply", "Voltage regulator output unstable", "Medium", "Power Validation", "Capacitor ESR exceeded limits on voltage regulator circuit"),
            ("Firmware", "Firmware checksum verification failed", "High", "Firmware Verification", "Flash memory write error during firmware update process"),
            ("Network", "Ethernet link negotiation failure", "Low", "Network Connectivity", "PHY chip intermittent failure on port 2")
        };

        foreach (var (failType, cause, severity, step, desc) in faultTypes)
        {
            var failedSession = sessions.FirstOrDefault(s => s.Status == "Failed");
            if (failedSession != null)
            {
                context.FaultRecords.Add(new FaultRecord
                {
                    TestSessionId = failedSession.Id,
                    FailureType = failType,
                    RootCause = cause,
                    Severity = severity,
                    StepName = step,
                    Description = desc,
                    OccurredAt = failedSession.StartTime.AddMinutes(Random.Shared.Next(5, 30)),
                    Resolution = severity == "Low" ? "Auto-retry resolved the issue" : null
                });
            }
        }
        context.SaveChanges();

        var testPlans = new[]
        {
            new TestPlan { Name = "Full Production Test", Description = "Complete production test sequence for all radio units", ProductType = "RBS 6601", Steps = "Initialize;RF Calibration;Power Validation;Temperature Test;Firmware Verification;Network Connectivity", CreatedDate = DateTime.Now.AddDays(-60) },
            new TestPlan { Name = "RF Calibration Only", Description = "Standalone RF calibration and verification", ProductType = "Radio 4478", Steps = "Initialize;RF Calibration", CreatedDate = DateTime.Now.AddDays(-45) },
            new TestPlan { Name = "Thermal Stress Test", Description = "Extended thermal cycling and monitoring test", ProductType = "Baseband 6630", Steps = "Initialize;Temperature Test", CreatedDate = DateTime.Now.AddDays(-30) },
            new TestPlan { Name = "Quick Validation", Description = "Rapid pass/fail validation for production line", ProductType = "Router 6274", Steps = "Initialize;Power Validation;Network Connectivity", CreatedDate = DateTime.Now.AddDays(-20) },
            new TestPlan { Name = "Firmware Verification", Description = "Firmware integrity and version verification", ProductType = "Antenna AIR 3246", Steps = "Initialize;Firmware Verification", CreatedDate = DateTime.Now.AddDays(-10) }
        };
        context.TestPlans.AddRange(testPlans);
        context.SaveChanges();
    }
}
