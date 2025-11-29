using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace QMS.Web.Hubs;

public class PrinterHub : Hub
{
    private static readonly ConcurrentDictionary<string, string> ConnectedPrinters = new();
    
    // Desktop Printer connects
    public async Task RegisterPrinter(string printerName, string location)
    {
        var connectionId = Context.ConnectionId;
        ConnectedPrinters[connectionId] = printerName;
        
        Console.WriteLine($"[PrinterHub] Printer '{printerName}' registered at {location} (ID: {connectionId})");
        
        // Notify all clients that a new printer is available
        await Clients.All.SendAsync("PrinterConnected", new
        {
            ConnectionId = connectionId,
            PrinterName = printerName,
            Location = location,
            Status = "Online"
        });
    }
    
    // Kiosk sends print request as JSON string
    public async Task BroadcastPrintJson(string jsonData)
    {
        Console.WriteLine($"[PrinterHub] Broadcasting JSON print command...");
        Console.WriteLine($"[PrinterHub] JSON: {jsonData}");
        
        if (ConnectedPrinters.Any())
        {
            // Send JSON string to all connected printers
            await Clients.All.SendAsync("PrintCommandJson", jsonData);
            Console.WriteLine($"[PrinterHub] JSON broadcasted to {ConnectedPrinters.Count} printer(s)");
        }
        else
        {
            Console.WriteLine($"[PrinterHub] No printers connected");
            await Clients.Caller.SendAsync("PrintError", "No printers available");
        }
    }
    
    // Kiosk sends print request to all printers (legacy)
    public async Task BroadcastPrint(object ticketData)
    {
        Console.WriteLine($"[PrinterHub] Broadcasting print command to all printers...");
        
        if (ConnectedPrinters.Any())
        {
            // Send to all connected printers
            await Clients.All.SendAsync("PrintCommand", ticketData);
            Console.WriteLine($"[PrinterHub] Print command broadcasted to {ConnectedPrinters.Count} printer(s)");
        }
        else
        {
            Console.WriteLine($"[PrinterHub] No printers connected");
            await Clients.Caller.SendAsync("PrintError", "No printers available");
        }
    }
    
    // Kiosk sends print request to specific printer
    public async Task PrintTicket(string printerConnectionId, object ticketData)
    {
        Console.WriteLine($"[PrinterHub] Print request for printer: {printerConnectionId}");
        
        if (ConnectedPrinters.ContainsKey(printerConnectionId))
        {
            // Send print command to specific printer
            await Clients.Client(printerConnectionId).SendAsync("PrintCommand", ticketData);
            Console.WriteLine($"[PrinterHub] Print command sent to {printerConnectionId}");
        }
        else
        {
            Console.WriteLine($"[PrinterHub] Printer {printerConnectionId} not found");
            await Clients.Caller.SendAsync("PrintError", "Printer not connected");
        }
    }
    
    // Desktop Printer confirms print completion
    public async Task PrintCompleted(string ticketId, bool success, string? errorMessage = null)
    {
        Console.WriteLine($"[PrinterHub] Print {(success ? "completed" : "failed")} for ticket {ticketId}");
        
        // Notify the kiosk that requested the print
        await Clients.All.SendAsync("PrintStatus", new
        {
            TicketId = ticketId,
            Success = success,
            ErrorMessage = errorMessage,
            Timestamp = DateTime.UtcNow
        });
    }
    
    // Get list of available printers
    public async Task<List<object>> GetAvailablePrinters()
    {
        var printers = ConnectedPrinters.Select(p => new
        {
            ConnectionId = p.Key,
            PrinterName = p.Value,
            Status = "Online"
        }).ToList<object>();
        
        return printers;
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        
        if (ConnectedPrinters.TryRemove(connectionId, out var printerName))
        {
            Console.WriteLine($"[PrinterHub] Printer '{printerName}' disconnected (ID: {connectionId})");
            
            // Notify all clients
            await Clients.All.SendAsync("PrinterDisconnected", new
            {
                ConnectionId = connectionId,
                PrinterName = printerName
            });
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}
