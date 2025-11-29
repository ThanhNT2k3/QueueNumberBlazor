using QMS.Domain.Interfaces;

namespace QMS.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly QmsDbContext _context;

    public UnitOfWork(QmsDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
