using Microsoft.EntityFrameworkCore;
using QMS.Domain.Common;
using QMS.Domain.Interfaces;
using QMS.Domain.Entities;
using System.Linq.Expressions;

namespace QMS.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly QmsDbContext _context;

    public Repository(QmsDbContext context)
    {
        _context = context;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().Where(predicate).ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsyncWithIncludes(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _context.Set<T>().Where(predicate);
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        // Special handling for Counter entity to include nested ServiceType
        if (typeof(T) == typeof(Counter))
        {
            query = ((IQueryable<Counter>)query)
                .Include("ServiceTypes.ServiceType") as IQueryable<T>;
        }
        
        return await query.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        await Task.CompletedTask;
    }
}
