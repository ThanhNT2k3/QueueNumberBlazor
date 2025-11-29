using Microsoft.EntityFrameworkCore;
using QMS.Domain.Entities;

namespace QMS.Infrastructure.Persistence;

public class QmsDbContext : DbContext
{
    public QmsDbContext(DbContextOptions<QmsDbContext> options) : base(options)
    {
    }

    public DbSet<Branch> Branches { get; set; }
    public DbSet<Counter> Counters { get; set; }
    public DbSet<ServiceType> ServiceTypes { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketLog> TicketLogs { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<CounterAssignmentHistory> CounterAssignmentHistories { get; set; }
    public DbSet<CounterServiceType> CounterServiceTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure entities if needed
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Branch)
            .WithMany()
            .HasForeignKey(t => t.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.ServiceType)
            .WithMany()
            .HasForeignKey(t => t.ServiceTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Staff)
            .WithMany()
            .HasForeignKey(t => t.StaffId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Counter>()
            .HasOne(c => c.Branch)
            .WithMany()
            .HasForeignKey(c => c.BranchId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Configure Counter-ServiceType many-to-many relationship
        modelBuilder.Entity<CounterServiceType>()
            .HasOne(cs => cs.Counter)
            .WithMany(c => c.ServiceTypes)
            .HasForeignKey(cs => cs.CounterId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<CounterServiceType>()
            .HasOne(cs => cs.ServiceType)
            .WithMany()
            .HasForeignKey(cs => cs.ServiceTypeId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Composite unique index to prevent duplicate service assignments
        modelBuilder.Entity<CounterServiceType>()
            .HasIndex(cs => new { cs.CounterId, cs.ServiceTypeId })
            .IsUnique();
    }
}
