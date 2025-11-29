using QMS.Application.Interfaces;
using QMS.Domain.Entities;
using QMS.Domain.Enums;
using QMS.Domain.Interfaces;

namespace QMS.Application.Services;

public class TicketService : ITicketService
{
    private readonly IRepository<Ticket> _ticketRepository;
    private readonly IRepository<Counter> _counterRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;
    private readonly IQmsNotificationService _notificationService;

    public TicketService(IRepository<Ticket> ticketRepository, IRepository<Counter> counterRepository, IUnitOfWork unitOfWork, IAuditService auditService, IQmsNotificationService notificationService)
    {
        _ticketRepository = ticketRepository;
        _counterRepository = counterRepository;
        _unitOfWork = unitOfWork;
        _auditService = auditService;
        _notificationService = notificationService;
    }

    public async Task<Ticket> CreateTicketAsync(int branchId, int serviceTypeId, string? language, string? customerName, string? customerPhone)
    {
        // Smart Route: Find best available counter for this service type
        var counters = await _counterRepository.FindAsync(c => c.BranchId == branchId && c.IsActive);
        var availableCounter = counters.OrderBy(c => c.DailySequence).FirstOrDefault();
        
        if (availableCounter == null)
        {
            throw new Exception("No available counters in this branch");
        }

        // Reset daily sequence if new day
        var today = DateTime.UtcNow.Date;
        if (availableCounter.LastResetDate < today)
        {
            availableCounter.DailySequence = 0;
            availableCounter.LastResetDate = today;
        }

        // Increment sequence
        availableCounter.DailySequence++;
        
        // Generate Ticket Number: [Prefix][yyMMdd][Sequence]
        // Example: C1251125001, VIP2511250001
        var ticketNumber = $"{availableCounter.Prefix}{availableCounter.DailySequence:D4}";
        
        var ticket = new Ticket
        {
            BranchId = branchId,
            ServiceTypeId = serviceTypeId,
            CounterId = availableCounter.Id, // Pre-assign to counter
            TicketNumber = ticketNumber,
            Language = language,
            CustomerName = customerName,
            CustomerPhone = customerPhone,
            Status = TicketStatus.Waiting
        };

        await _ticketRepository.AddAsync(ticket);
        await _counterRepository.UpdateAsync(availableCounter);
        await _unitOfWork.SaveChangesAsync();
        
        // Notify realtime
        await _notificationService.NotifyTicketUpdatedAsync(branchId, availableCounter.Id);

        return ticket;
    }

    public async Task<Ticket?> GetTicketByIdAsync(int id)
    {
        return await _ticketRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Ticket>> GetWaitingTicketsAsync(int branchId, int counterId)
    {
        return await _ticketRepository.FindAsync(t => t.BranchId == branchId && t.CounterId == counterId && t.Status == TicketStatus.Waiting);
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByCounterAsync(int counterId)
    {
        return await _ticketRepository.FindAsync(t => t.CounterId == counterId && 
            (t.Status == TicketStatus.Called || t.Status == TicketStatus.Serving));
    }

    public async Task<Ticket?> CallNextTicketAsync(int counterId, int? staffId = null)
    {
        var counter = await _counterRepository.GetByIdAsync(counterId);
        if (counter == null) return null;

        // 1. Priority: Get ticket assigned specifically to this counter
        var tickets = await _ticketRepository.FindAsync(t => t.CounterId == counterId && t.BranchId == counter.BranchId && t.Status == TicketStatus.Waiting);
        var nextTicket = tickets.OrderBy(t => t.CreatedAt).FirstOrDefault();

        // 2. Backup Logic: If no specific ticket, find ANY waiting ticket in the branch 
        // that is NOT assigned to any other active counter (or is assigned to this counter which we checked above)
        // For simplicity in this "Backup" implementation:
        // We look for tickets that are waiting and have NO CounterId assigned yet (if your flow supports unassigned tickets)
        // OR tickets assigned to this counter (already covered)
        // OR tickets assigned to counters that are currently OFFLINE (Rescue mission!)
        
        if (nextTicket == null)
        {
            // Find all waiting tickets in branch
            var allWaitingTickets = await _ticketRepository.FindAsync(t => t.BranchId == counter.BranchId && t.Status == TicketStatus.Waiting);
            
            // Get IDs of all currently ONLINE counters (excluding this one)
            var onlineCounters = await _counterRepository.FindAsync(c => c.BranchId == counter.BranchId && c.Id != counterId && c.Status == CounterStatus.Online);
            var onlineCounterIds = onlineCounters.Select(c => c.Id).ToList();

            // Find a ticket that is EITHER:
            // - Not assigned to any counter (CounterId is null)
            // - Assigned to a counter that is NOT online (Rescue)
            nextTicket = allWaitingTickets
                .Where(t => !t.CounterId.HasValue || !onlineCounterIds.Contains(t.CounterId.Value))
                .OrderBy(t => t.CreatedAt) // FIFO
                .FirstOrDefault();
        }

        if (nextTicket != null)
        {
            nextTicket.Status = TicketStatus.Called;
            nextTicket.CounterId = counterId; // Assign to this counter
            nextTicket.CalledAt = DateTime.UtcNow;
            nextTicket.StartServiceTime = DateTime.UtcNow;
            nextTicket.StaffId = staffId;
            
            counter.CurrentTicketId = nextTicket.Id;
            
            await _ticketRepository.UpdateAsync(nextTicket);
            await _counterRepository.UpdateAsync(counter);
            await _unitOfWork.SaveChangesAsync();
            
            // Notify realtime
            await _notificationService.NotifyTicketUpdatedAsync(counter.BranchId, counterId);
            await _notificationService.NotifyCounterUpdatedAsync(counterId);
        }

        return nextTicket;
    }

    public async Task<Ticket?> RecallTicketAsync(int ticketId)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket != null)
        {
            ticket.Status = TicketStatus.Called; // Re-call
            ticket.CalledAt = DateTime.UtcNow;
            await _ticketRepository.UpdateAsync(ticket);
            await _unitOfWork.SaveChangesAsync();
            
            // Notify realtime
            await _notificationService.NotifyTicketUpdatedAsync(ticket.BranchId, ticket.CounterId.Value);
        }
        return ticket;
    }

    public async Task CompleteTicketAsync(int ticketId)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket != null)
        {
            ticket.Status = TicketStatus.Completed;
            ticket.CompletedAt = DateTime.UtcNow;
            ticket.EndServiceTime = DateTime.UtcNow;
            
            if (ticket.CounterId.HasValue)
            {
                var counter = await _counterRepository.GetByIdAsync(ticket.CounterId.Value);
                if (counter != null)
                {
                    counter.CurrentTicketId = null;
                    await _counterRepository.UpdateAsync(counter);
                }
            }
            
            await _ticketRepository.UpdateAsync(ticket);
            await _unitOfWork.SaveChangesAsync();
            
            // Notify realtime
            if (ticket.CounterId.HasValue)
            {
                await _notificationService.NotifyTicketUpdatedAsync(ticket.BranchId, ticket.CounterId.Value);
                await _notificationService.NotifyCounterUpdatedAsync(ticket.CounterId.Value);
            }
        }
    }

    public async Task MissTicketAsync(int ticketId)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket != null)
        {
            ticket.Status = TicketStatus.Missed;
            await _ticketRepository.UpdateAsync(ticket);
            await _unitOfWork.SaveChangesAsync();
            
            // Notify realtime
            if (ticket.CounterId.HasValue)
            {
                await _notificationService.NotifyTicketUpdatedAsync(ticket.BranchId, ticket.CounterId.Value);
            }
        }
    }

    public async Task TransferTicketAsync(int ticketId, int targetCounterId, string? note)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket != null)
        {
            var oldCounterId = ticket.CounterId;
            ticket.CounterId = targetCounterId;
            ticket.Status = TicketStatus.Waiting; // Put back to wait or directly call? Usually wait for target counter.
            // Or if direct transfer, maybe "Transferred" status? Let's keep Waiting but assigned.
            
            await _ticketRepository.UpdateAsync(ticket);
            await _auditService.LogAsync("SYSTEM", "System", AuditAction.TransferCounter, "Ticket", ticket.Id.ToString(), oldCounterId?.ToString(), targetCounterId.ToString(), note);
            await _unitOfWork.SaveChangesAsync();
            
            // Notify realtime
            if (oldCounterId.HasValue)
            {
                await _notificationService.NotifyTicketUpdatedAsync(ticket.BranchId, oldCounterId.Value);
            }
            await _notificationService.NotifyTicketUpdatedAsync(ticket.BranchId, targetCounterId);
        }
    }
    public async Task UpdateTicketInfoAsync(int ticketId, string? customerPhone, string? customerEmail, string? customerNotes, string? remarks)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket != null)
        {
            ticket.CustomerPhone = customerPhone;
            ticket.CustomerEmail = customerEmail;
            ticket.CustomerNotes = customerNotes;
            ticket.Remarks = remarks;
            
            await _ticketRepository.UpdateAsync(ticket);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Ticket>> GetTicketHistoryByCounterAsync(int counterId)
    {
        // Get tickets that are Completed or Missed for this counter, ordered by latest first
        var tickets = await _ticketRepository.FindAsync(t => t.CounterId == counterId && 
            (t.Status == TicketStatus.Completed || t.Status == TicketStatus.Missed));
            
        return tickets.OrderByDescending(t => t.UpdatedAt).ToList();
    }

    // TM Dashboard Methods
    public async Task<IEnumerable<Ticket>> GetTicketsByFiltersAsync(
        int branchId, 
        DateTime? fromDate = null, 
        DateTime? toDate = null, 
        int? counterId = null, 
        int? userId = null)
    {
        var tickets = await _ticketRepository.FindAsyncWithIncludes(
            t => t.BranchId == branchId,
            t => t.ServiceType!,
            t => t.Counter!,
            t => t.Staff!
        );
        
        var filtered = tickets.AsEnumerable();

        if (fromDate.HasValue)
        {
            var fromDateUtc = fromDate.Value.ToUniversalTime();
            filtered = filtered.Where(t => t.CreatedAt >= fromDateUtc);
        }

        if (toDate.HasValue)
        {
            var toDateUtc = toDate.Value.ToUniversalTime().AddDays(1); // Include entire day
            filtered = filtered.Where(t => t.CreatedAt < toDateUtc);
        }

        if (counterId.HasValue)
        {
            filtered = filtered.Where(t => t.CounterId == counterId.Value);
        }

        // Note: To filter by userId (staff), we need to track which user served the ticket
        // This would require adding a ServedByUserId field to Ticket entity
        // For now, we can filter by counter's assigned user indirectly
        
        return filtered.OrderByDescending(t => t.CreatedAt).ToList();
    }
}
