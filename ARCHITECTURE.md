# Architecture Overview

This document provides a technical overview of the Queue Management System (QMS) architecture.

## 🛠 Technology Stack

- **Framework**: .NET 8.0
- **UI Framework**: Blazor Server
- **Database**: SQLite (via Entity Framework Core)
- **Real-time Communication**: SignalR
- **Architecture Pattern**: Clean Architecture (Onion Architecture)

## 🏗 Solution Structure

The solution is divided into 5 main projects:

### 1. QMS.Domain
*The core of the application, containing enterprise logic and entities.*
- **Entities**:
  - `Ticket`: Represents a customer queue ticket.
  - `Counter`: Represents a service counter.
  - `ServiceType`: Represents a type of service (e.g., General Inquiry, Deposit).
  - `User`: Represents a system user (Admin, Manager, Teller).
  - `Branch`: Represents a physical branch location.
  - `AuditLog`: Tracks system activities.
- **Interfaces**: Domain-level interfaces.

### 2. QMS.Application
*Contains application business logic.*
- **Interfaces**:
  - `IUnitOfWork`: For transaction management.
  - `IRepository<T>`: Generic repository interface.
  - `IQmsNotificationService`: Interface for real-time notifications.
- **Services**: Business logic services (if any separate from UI).

### 3. QMS.Infrastructure
*Implements interfaces defined in Application and Domain.*
- **Persistence**:
  - `QmsDbContext`: EF Core database context.
  - `Repository<T>`: Generic repository implementation.
  - `UnitOfWork`: Implementation of unit of work pattern.
- **Dependency Injection**: Registers infrastructure services.

### 4. QMS.Web
*The entry point and UI of the application.*
- **Components**: Blazor components for Pages, Layouts, and shared UI.
- **Hubs**:
  - `QmsHub`: Manages real-time connections for Dashboard and Displays.
  - `PrinterHub`: Manages connections for Printer Clients.
- **Services**:
  - `QmsNotificationService`: Wraps SignalR calls to notify clients.
  - `AuthenticationStateService`: Manages user session state.

### 5. QMS.PrinterClient
*A standalone console application for hardware integration.*
- **Role**: Connects to `PrinterHub` to receive print jobs.
- **Functionality**:
  - Registers with the server using a Printer Name and Location.
  - Listens for `PrintCommand` and `PrintCommandJson`.
  - Simulates thermal printing (console output).
  - Reports print completion status back to the server.

## 📡 Real-time Data Flow

### Ticket Creation (Kiosk)
1. User clicks a service on Kiosk.
2. `TicketService` creates a new Ticket in DB.
3. `QmsNotificationService` sends `TicketUpdated` event via `QmsHub`.
4. **Dashboard** and **Counter Display** receive event and refresh their lists.

### Ticket Printing
1. Kiosk requests ticket print.
2. Server identifies available Printer Client.
3. Server sends `PrintCommand` via `PrinterHub` to the specific connection ID of the printer.
4. **Printer Client** receives data, prints ticket, and sends `PrintCompleted`.

### Counter Status Change
1. Teller changes status (e.g., Online -> Busy).
2. `QmsNotificationService` sends `CounterStatusChanged`.
3. **Dashboard** updates the counter's status indicator.
4. **Counter Display** updates its availability status.

## 💾 Database Schema (SQLite)

The application uses a relational model with the following key relationships:
- **Branch** 1-to-Many **Counter**
- **Counter** Many-to-Many **ServiceType** (via `CounterServiceType`)
- **Ticket** Many-to-1 **ServiceType**
- **Ticket** Many-to-1 **Counter** (nullable, assigned when serving)
- **Ticket** Many-to-1 **User** (nullable, processed by)

## 🔒 Security

- **Authentication**: Cookie-based authentication.
- **Authorization**: Role-based access control (Admin, Manager, Teller).
- **SignalR**: Hubs are currently open but can be secured with standard Authentication schemes.
