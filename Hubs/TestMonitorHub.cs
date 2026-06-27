using Microsoft.AspNetCore.SignalR;

namespace WebApplication.Hubs;

public class TestMonitorHub : Hub
{
    public async Task SendTestEvent(string eventType, string message, string? stationName = null)
    {
        await Clients.All.SendAsync("ReceiveTestEvent", eventType, message, stationName, DateTime.Now);
    }

    public async Task SendStationUpdate(string stationName, string status, string? currentTest = null)
    {
        await Clients.All.SendAsync("ReceiveStationUpdate", stationName, status, currentTest, DateTime.Now);
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("ReceiveTestEvent", "System", "Connected to monitoring hub", null, DateTime.Now);
        await base.OnConnectedAsync();
    }
}
