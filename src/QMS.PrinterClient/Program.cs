using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.Text;

namespace QMS.PrinterClient;

class Program
{
    private static HubConnection? _connection;
    private static string _printerName = "Kiosk-Printer-01";
    private static string _location = "Branch 1 - Main Entrance";
    private static int _branchId = 0; // 0 means not set

    static async Task Main(string[] args)
    {
        Console.WriteLine("╔═══════════════════════════════════════╗");
        Console.WriteLine("║   QMS Thermal Printer Client v1.0     ║");
        Console.WriteLine("╚═══════════════════════════════════════╝");
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine();

        // Allow custom printer name
        if (args.Length > 0)
        {
            _printerName = args[0];
        }
        if (args.Length > 1)
        {
            _location = args[1];
        }
        if (args.Length > 2 && int.TryParse(args[2], out int branchId))
        {
            _branchId = branchId;
        }

        await ConnectToPrinterHub();

        // If Branch ID is not set, ask user to select from database
        if (_branchId == 0)
        {
            await SelectBranch();
        }

        Console.WriteLine($"Printer Name: {_printerName}");
        Console.WriteLine($"Location: {_location}");
        Console.WriteLine($"Branch ID: {_branchId}");
        Console.WriteLine();

        await RegisterPrinter();

        Console.WriteLine("\nPress 'Q' to quit...");
        while (Console.ReadKey(true).Key != ConsoleKey.Q)
        {
            await Task.Delay(100);
        }

        await _connection?.StopAsync()!;
    }

    static async Task ConnectToPrinterHub()
    {
        // Get URL from environment variable or default to localhost
        var envUrl = Environment.GetEnvironmentVariable("QMS_SERVER_URL");
        var serverUrl = !string.IsNullOrEmpty(envUrl) ? envUrl : "http://localhost:5101/printerhub";
        
        Console.WriteLine($"Connecting to {serverUrl}...");

        _connection = new HubConnectionBuilder()
            .WithUrl(serverUrl)
            .WithAutomaticReconnect()
            .Build();

        // Handle print commands from server (JSON string)
        _connection.On<string>("PrintCommandJson", async (jsonData) =>
        {
            await HandlePrintCommandJson(jsonData);
        });
        
        // Handle print commands from server (object - legacy)
        _connection.On<object>("PrintCommand", async (ticketData) =>
        {
            await HandlePrintCommand(ticketData);
        });

        // Handle reconnection
        _connection.Reconnecting += error =>
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Connection lost. Reconnecting...");
            return Task.CompletedTask;
        };

        _connection.Reconnected += connectionId =>
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Reconnected! ID: {connectionId}");
            return RegisterPrinter();
        };

        _connection.Closed += error =>
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Connection closed: {error?.Message}");
            return Task.CompletedTask;
        };

        try
        {
            await _connection.StartAsync();
            Console.WriteLine($"✓ Connected successfully! ID: {_connection.ConnectionId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Connection failed: {ex.Message}");
            Console.WriteLine("Please check if the server is running.");
            Environment.Exit(1);
        }
    }

    static async Task SelectBranch()
    {
        try
        {
            Console.WriteLine("Fetching available branches...");
            var branches = await _connection!.InvokeAsync<List<dynamic>>("GetBranches");

            if (branches == null || branches.Count == 0)
            {
                Console.WriteLine("No branches found in database. Defaulting to Branch ID 1.");
                _branchId = 1;
                return;
            }

            Console.WriteLine("\nAvailable Branches:");
            Console.WriteLine("═══════════════════════════════════════");
            for (int i = 0; i < branches.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] {branches[i].Name} (Code: {branches[i].Code})");
            }
            Console.WriteLine("═══════════════════════════════════════");

            while (true)
            {
                Console.Write("\nSelect a branch (enter number): ");
                string? input = Console.ReadLine();

                if (int.TryParse(input, out int selection) && selection > 0 && selection <= branches.Count)
                {
                    var selectedBranch = branches[selection - 1];
                    _branchId = (int)selectedBranch.Id;
                    _location = $"{selectedBranch.Name} - Main Entrance"; // Auto-update location based on branch
                    Console.WriteLine($"✓ Selected: {selectedBranch.Name}");
                    break;
                }
                Console.WriteLine("Invalid selection. Please try again.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching branches: {ex.Message}");
            Console.WriteLine("Defaulting to Branch ID 1.");
            _branchId = 1;
        }
    }

    static async Task RegisterPrinter()
    {
        try
        {
            await _connection!.InvokeAsync("RegisterPrinter", _printerName, _location, _branchId);
            Console.WriteLine($"✓ Printer registered: {_printerName} (Branch: {_branchId})");
            Console.WriteLine("Waiting for print jobs...\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Registration failed: {ex.Message}");
        }
    }

    static async Task HandlePrintCommandJson(string jsonData)
    {
        try
        {
            Console.WriteLine("\n" + new string('═', 50));
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] NEW PRINT JOB RECEIVED (JSON)");
            Console.WriteLine(new string('═', 50));
            // Console.WriteLine("RAW JSON DATA:");
            // Console.WriteLine(jsonData);
            Console.WriteLine(new string('-', 50));
            
            dynamic? ticket = JsonConvert.DeserializeObject(jsonData);
            
            // Simulate thermal printer output
            PrintTicket(ticket);
            
            // Simulate printing delay
            await Task.Delay(2000);
            
            // Notify server that printing is complete
            string ticketId = ticket?.Id?.ToString() ?? "unknown";
            await _connection!.InvokeAsync("PrintCompleted", ticketId, true, null);
            
            Console.WriteLine("\n✓ Print job completed successfully!");
            Console.WriteLine(new string('═', 50) + "\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Print error: {ex.Message}\n");
            Console.WriteLine($"Stack trace: {ex.StackTrace}\n");
        }
    }

    static async Task HandlePrintCommand(object ticketData)
    {
        try
        {
            var json = JsonConvert.SerializeObject(ticketData, Formatting.Indented);
            
            Console.WriteLine("\n" + new string('═', 50));
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] NEW PRINT JOB RECEIVED");
            Console.WriteLine(new string('═', 50));
            // Console.WriteLine("RAW JSON DATA:");
            // Console.WriteLine(json);
            Console.WriteLine(new string('-', 50));
            
            dynamic? ticket = JsonConvert.DeserializeObject(json);
            
            // Simulate thermal printer output
            PrintTicket(ticket);
            
            // Simulate printing delay
            await Task.Delay(2000);
            
            // Notify server that printing is complete
            string ticketId = ticket?.Id?.ToString() ?? "unknown";
            await _connection!.InvokeAsync("PrintCompleted", ticketId, true, null);
            
            Console.WriteLine("\n✓ Print job completed successfully!");
            Console.WriteLine(new string('═', 50) + "\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Print error: {ex.Message}\n");
            
            try
            {
                dynamic? ticket = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(ticketData));
                string ticketId = ticket?.Id?.ToString() ?? "unknown";
                await _connection!.InvokeAsync("PrintCompleted", ticketId, false, ex.Message);
            }
            catch { }
        }
    }

    static void PrintTicket(dynamic? ticket)
    {
        if (ticket == null) return;

        var sb = new StringBuilder();
        
        // Thermal printer format (48 characters wide)
        sb.AppendLine();
        sb.AppendLine(Center("STANDARD CHARTERED BANK"));
        sb.AppendLine(Center("Queue Management System"));
        sb.AppendLine(new string('-', 48));
        sb.AppendLine();
        
        // Ticket Number (Large)
        string ticketNumber = ticket.TicketNumber?.ToString() ?? "N/A";
        sb.AppendLine(Center("YOUR TICKET NUMBER"));
        sb.AppendLine();
        sb.AppendLine(Center($"*** {ticketNumber} ***", large: true));
        sb.AppendLine();
        sb.AppendLine(new string('-', 48));
        
        // Service Info - Read ServiceName directly
        string serviceName = ticket.ServiceName?.ToString() ?? "N/A";
        sb.AppendLine($"Service: {serviceName}");
        
        // Counter Info - Read CounterName directly
        string counterName = ticket.CounterName?.ToString() ?? "Waiting...";
        sb.AppendLine($"Counter: {counterName}");
        
        // Timestamp
        string createdAt = ticket.CreatedAt?.ToString() ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        sb.AppendLine($"Time: {createdAt}");
        
        sb.AppendLine(new string('-', 48));
        sb.AppendLine();
        sb.AppendLine(Center("Please wait for your number"));
        sb.AppendLine(Center("to be called"));
        sb.AppendLine();
        sb.AppendLine(Center("Thank you for your patience!"));
        sb.AppendLine();
        sb.AppendLine(new string('=', 48));
        
        // Print to console (simulating thermal printer)
        Console.WriteLine(sb.ToString());
    }

    static string Center(string text, bool large = false)
    {
        int width = large ? 24 : 48;
        int padding = Math.Max(0, (width - text.Length) / 2);
        return new string(' ', padding) + text;
    }
}
