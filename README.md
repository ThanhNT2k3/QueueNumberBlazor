# Queue Management System (QMS)

A comprehensive Queue Management System built with Blazor Server, featuring real-time updates, a ticket kiosk, counter management, and a thermal printer client.

## 🏗 Project Structure

The solution follows a Clean Architecture approach:

- **QMS.Web**: The main Blazor Server application containing the UI and SignalR hubs.
- **QMS.PrinterClient**: A console application that acts as a client for thermal printers, receiving print jobs via SignalR.
- **QMS.Domain**: Contains the core entities (Ticket, Counter, ServiceType, etc.).
- **QMS.Infrastructure**: Handles data persistence (EF Core) and external services.
- **QMS.Application**: Contains business logic and interfaces.

## 🚀 Key Features

### 1. Web Application (`QMS.Web`)
- **Dashboard**: Real-time dashboard for managers and tellers to view and manage tickets.
- **Kiosk Interface**: A user-friendly interface for customers to select services and generate tickets.
- **Counter Display**: A public-facing display showing the current ticket being served and the waiting list.
- **Real-time Updates**: Uses SignalR (`QmsHub`) to push updates instantly across all connected clients (Dashboard, Display, Kiosk).

### 2. Printer Client (`QMS.PrinterClient`)
- **Remote Printing**: Connects to the server via SignalR (`PrinterHub`) to receive print commands.
- **Thermal Printer Simulation**: Currently simulates printing output to the console (can be extended to actual hardware).
- **Auto-Reconnect**: Automatically reconnects to the server if the connection is lost.

## 🛠 Getting Started

### Prerequisites
- .NET 8.0 SDK or later

### Running the Web Application
1. Navigate to the `src/QMS.Web` directory.
2. Run the application:
   ```bash
   dotnet run
   ```
3. The application will start at `http://localhost:5101`.

### Running the Printer Client
1. Open a new terminal window.
2. Navigate to the `src/QMS.PrinterClient` directory.
3. Run the client (optionally specify printer name and location):
   ```bash
   dotnet run -- "MyPrinter" "Main Lobby"
   ```
4. The client will connect to the web server and wait for print jobs.

## 📡 Real-time Communication

The system relies heavily on SignalR for real-time functionality:

- **QmsHub**: Handles updates for tickets, counters, and user status.
  - Clients: Dashboard, Counter Display.
- **PrinterHub**: Handles communication with the Printer Client.
  - Clients: Printer Console App.

## 🔍 Debugging & Troubleshooting

For detailed instructions on debugging real-time synchronization issues, please refer to [REALTIME_DEBUG_GUIDE.md](REALTIME_DEBUG_GUIDE.md).

### Common Issues
- **Logs not showing**: Check `appsettings.Development.json` logging levels.
- **Printer not connecting**: Ensure the Web App is running before starting the Printer Client.

## 📝 Recent Updates
- Implemented `PrinterHub` for remote ticket printing.
- Added `QMS.PrinterClient` console application.
- Enhanced `QmsNotificationService` for robust real-time events.
- Fixed Counter Display standalone mode and sync issues.
