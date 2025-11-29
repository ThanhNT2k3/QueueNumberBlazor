using QMS.Domain.Entities;
using QMS.Domain.Enums;

namespace QMS.Application.Interfaces;

public interface ITicketService
{
    Task<Ticket> CreateTicketAsync(int branchId, int serviceTypeId, string? language, string? customerName, string? customerPhone);
    Task<Ticket?> GetTicketByIdAsync(int id);
    Task<IEnumerable<Ticket>> GetWaitingTicketsAsync(int branchId , int counterId);
    Task<IEnumerable<Ticket>> GetTicketsByCounterAsync(int counterId);
    Task<Ticket?> CallNextTicketAsync(int counterId, int? staffId = null);
    Task<Ticket?> RecallTicketAsync(int ticketId);
    Task CompleteTicketAsync(int ticketId);
    Task MissTicketAsync(int ticketId);
    Task UpdateTicketInfoAsync(int ticketId, string? customerPhone, string? customerEmail, string? customerNotes, string? remarks);
    Task<IEnumerable<Ticket>> GetTicketHistoryByCounterAsync(int counterId);
    Task TransferTicketAsync(int ticketId, int targetCounterId, string? note);
    
    // TM Dashboard Methods
    Task<IEnumerable<Ticket>> GetTicketsByFiltersAsync(int branchId, DateTime? fromDate = null, DateTime? toDate = null, int? counterId = null, int? userId = null);
}
