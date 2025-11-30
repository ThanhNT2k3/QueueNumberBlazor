using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace QMS.Web.Hubs;

public class PrinterHub : Hub
{
    private static readonly ConcurrentDictionary<string, PrinterInfo> ConnectedPrinters = new();
    private readonly QMS.Domain.Interfaces.IRepository<QMS.Domain.Entities.Branch> _branchRepository;

    public PrinterHub(QMS.Domain.Interfaces.IRepository<QMS.Domain.Entities.Branch> branchRepository)
    {
        _branchRepository = branchRepository;
    }
    
    public class PrinterInfo
    {
        public string PrinterName { get; set; } = "";
        public string Location { get; set; } = "";
        public int BranchId { get; set; }
    }

    // Get list of all branches
    public async Task<List<object>> GetBranches()
    {
        var branches = await _branchRepository.GetAllAsync();
        return branches.Select(b => new 
        { 
            Id = b.Id, 
            Name = b.Name,
            Code = b.Code
        }).ToList<object>();
    }
    
    // Desktop Printer connects
    public async Task RegisterPrinter(string printerName, string location, int branchId)
    {
        var connectionId = Context.ConnectionId;
        ConnectedPrinters[connectionId] = new PrinterInfo 
        { 
            PrinterName = printerName, 
            Location = location, 
            BranchId = branchId 
        };
        
        Console.WriteLine($"[PrinterHub] Printer '{printerName}' registered at {location} (Branch: {branchId}, ID: {connectionId})");
        
        // Notify all clients that a new printer is available
        await Clients.All.SendAsync("PrinterConnected", new
        {
            ConnectionId = connectionId,
            PrinterName = printerName,
            Location = location,
            BranchId = branchId,
            Status = "Online"
        });
    }
    
    // Kiosk sends print request as JSON string
    public async Task BroadcastPrintJson(string jsonData, int branchId)
    {
        Console.WriteLine($"[PrinterHub] Broadcasting JSON print command to Branch {branchId}...");
        Console.WriteLine($"[PrinterHub] JSON: {jsonData}");
        
        var targetPrinters = ConnectedPrinters.Where(p => p.Value.BranchId == branchId).Select(p => p.Key).ToList();
        
        if (targetPrinters.Any())
        {
            // Send JSON string to connected printers in the specific branch
            await Clients.Clients(targetPrinters).SendAsync("PrintCommandJson", jsonData);
            Console.WriteLine($"[PrinterHub] JSON broadcasted to {targetPrinters.Count} printer(s) in Branch {branchId}");
        }
        else
        {
            Console.WriteLine($"[PrinterHub] No printers connected for Branch {branchId}");
            await Clients.Caller.SendAsync("PrintError", $"No printers available for Branch {branchId}");
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
            PrinterName = p.Value.PrinterName,
            Location = p.Value.Location,
            BranchId = p.Value.BranchId,
            Status = "Online"
        }).ToList<object>();
        
        return printers;
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        
        if (ConnectedPrinters.TryRemove(connectionId, out var printerInfo))
        {
            Console.WriteLine($"[PrinterHub] Printer '{printerInfo.PrinterName}' disconnected (ID: {connectionId})");
            
            // Notify all clients
            await Clients.All.SendAsync("PrinterDisconnected", new
            {
                ConnectionId = connectionId,
                PrinterName = printerInfo.PrinterName
            });
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}
