# C# Factory Test Hub

**Production Test Management System Demo**

A full-stack Blazor Server application that simulates a factory production test management system for radio and telecom equipment. Designed to demonstrate enterprise-grade C# development patterns including real-time monitoring, data-driven dashboards, and structured test execution workflows.

---

## Key Features

- **Production Dashboard** -- KPI widgets, test throughput charts, pass/fail analytics, and recent session tracking
- **Test Execution Engine** -- Configure and run simulated test sequences with live progress indicators
- **Hardware Simulator** -- Connect to mock radio devices, read telemetry data, and execute individual test steps
- **Real-Time Monitoring** -- Live SignalR event stream with station status updates and running execution feeds
- **Fault Analysis** -- Failure records with severity and type filters, root cause tracking, and session drill-down
- **Report Generation** -- Test execution reports with structured data export

## System Architecture

```
┌─────────────────────────────────────────────────────┐
│                   Blazor Server UI                  │
│          (MudBlazor Material Design Components)     │
├──────────────┬──────────────────┬───────────────────┤
│  Dashboard   │  Test Execution  │   Fault Analysis  │
│  Monitoring  │  HW Simulator    │   Documentation   │
├──────────────┴──────────────────┴───────────────────┤
│                  Service Layer                      │
│   IDashboardService    ITestSessionService          │
│   IFaultAnalysisService    ITestDeviceService       │
├─────────────────────────────────────────────────────┤
│              SignalR Hub (TestMonitorHub)            │
├─────────────────────────────────────────────────────┤
│            Data Access (EF Core + SQLite)            │
│                    AppDbContext                      │
└─────────────────────────────────────────────────────┘
```

**Layers:**

| Layer | Responsibility |
|-------|---------------|
| **UI** | Blazor components with MudBlazor for Material Design rendering |
| **Services** | Business logic with interface-based dependency injection |
| **SignalR** | Real-time bidirectional communication for monitoring |
| **Data** | Entity Framework Core with SQLite, auto-seeded demo data |

## Tech Stack

| Technology | Purpose |
|-----------|---------|
| **.NET 10** | Runtime and framework |
| **Blazor Server** | Interactive server-side rendering with SignalR circuit |
| **MudBlazor 8.x** | Material Design UI component library |
| **SignalR** | Real-time event streaming and station monitoring |
| **Entity Framework Core** | ORM and data access |
| **SQLite** | Lightweight embedded database (auto-created on first run) |
| **Dependency Injection** | Service registration via built-in DI container |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later

## Installation

```bash
git clone https://github.com/<your-username>/csharp-factory-test-hub.git
cd csharp-factory-test-hub
```

## How to Run Locally

```bash
dotnet restore
dotnet run
```

Navigate to the URL shown in console output (typically `https://localhost:5001` or `http://localhost:5000`).
## Output - Screens
**Dashboard:-**
<img width="1920" height="1524" alt="image" src="https://github.com/user-attachments/assets/bda5886a-3597-4742-81fb-9a00363c739c" />
**Execute Test:-**
<img width="1920" height="1204" alt="image" src="https://github.com/user-attachments/assets/c5dc2f49-e370-4e81-a301-08b2357ee0d4" />
**Hardware Simulator:-**
<img width="1920" height="1091" alt="image" src="https://github.com/user-attachments/assets/88f5190a-6803-4ced-acdb-8ad5701afbdf" />
**Monitoring:-**
<img width="1920" height="1136" alt="image" src="https://github.com/user-attachments/assets/9215a46a-4061-493a-a08b-0250b8a17954" />
**Falut Analysis:-**
<img width="1920" height="1266" alt="image" src="https://github.com/user-attachments/assets/89d22b67-8cc8-4c58-83f4-a39eb8ba506f" />
**Documentation:-**
<img width="1920" height="1402" alt="image" src="https://github.com/user-attachments/assets/12a33086-733c-48ae-9ab5-76c555c1f34b" />

The SQLite database is created and seeded automatically on first launch with 30 devices, 10 test sessions, 5 production stations, and 5 fault records.

## Usage Guide

| Page | Route | Description |
|------|-------|-------------|
| **Dashboard** | `/` | KPI widgets, throughput and pass/fail charts, recent sessions table |
| **Execute Test** | `/tests` | Configure test parameters and run simulated sequences with live progress |
| **HW Simulator** | `/simulator` | Connect to mock devices, read telemetry, execute individual test steps |
| **Monitoring** | `/monitor` | Real-time SignalR event stream, station status, running executions |
| **Fault Analysis** | `/analysis` | Fault records with severity/type filters, failed session drill-down |
| **Documentation** | `/docs` | Generate and view test execution reports |

## Project Structure

```
WebApplication/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor          # App shell with responsive nav and dark mode
│   │   └── NavMenu.razor             # Side navigation menu
│   ├── Pages/
│   │   ├── Dashboard.razor           # Production monitoring dashboard
│   │   ├── ExecuteTest.razor         # Test execution with live progress
│   │   ├── HardwareSimulator.razor   # Device simulator with telemetry
│   │   ├── Monitoring.razor          # Real-time SignalR event stream
│   │   ├── FaultAnalysis.razor       # Failure analysis with filters
│   │   └── Documentation.razor       # Report generation
│   ├── App.razor
│   ├── Routes.razor
│   └── _Imports.razor
├── Data/
│   ├── AppDbContext.cs               # EF Core database context (7 entities)
│   └── SeedData.cs                   # Demo data seeding
├── Hubs/
│   └── TestMonitorHub.cs             # SignalR hub for real-time events
├── Models/
│   ├── Device.cs                     # Radio/telecom device entity
│   ├── TestPlan.cs                   # Test plan configuration
│   ├── TestSession.cs                # Test execution session
│   ├── TestStep.cs                   # Individual test step with measurements
│   ├── ExecutionLog.cs               # Execution log entry
│   ├── FaultRecord.cs                # Fault/failure record
│   └── ProductionStation.cs          # Factory production station
├── Services/
│   ├── IDashboardService.cs          # Dashboard data contract
│   ├── DashboardService.cs           # Dashboard KPI and chart data
│   ├── ITestSessionService.cs        # Test session operations contract
│   ├── TestSessionService.cs         # Test session management
│   ├── IFaultAnalysisService.cs      # Fault analysis contract
│   ├── FaultAnalysisService.cs       # Fault querying and filtering
│   ├── ITestDeviceService.cs         # Hardware abstraction interface
│   └── MockRadioDeviceService.cs     # Simulated device with random delays/failures
├── wwwroot/
│   ├── css/app.css                   # Custom styles
│   └── images/logo.png               # Application logo
├── Program.cs                        # Application entry point and DI configuration
├── appsettings.json                  # Application configuration
└── WebApplication.csproj             # Project file and NuGet dependencies
```

## Design Patterns

- **Service Layer** -- Business logic isolated in injectable services behind interfaces
- **Interface Segregation** -- Separate contracts per domain: `IDashboardService`, `ITestSessionService`, `IFaultAnalysisService`, `ITestDeviceService`
- **Mock Hardware Abstraction** -- `MockRadioDeviceService` simulates real device behavior with configurable delays and failure rates
- **Repository via EF Core** -- Data access through `AppDbContext` with navigation properties and relationship configuration
- **Real-Time Communication** -- SignalR hub broadcasting test events and station status updates
- **Code-First Database** -- Auto-created schema with `EnsureCreated()` and deterministic seed data

## Future Improvements

- Add authentication and role-based access control
- Implement PDF export for test reports
- Add unit and integration test coverage
- Support multiple database providers (SQL Server, PostgreSQL)
- Containerize with Docker for deployment

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.
