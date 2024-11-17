using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OS.Data.Context;
using OS.Data.Models;
using OS.Data.Repository;
using OS.Data.Repository.Conditions;

namespace OS.Services.Repository;

public class SqlRepository(OsDbContext context) : IRepository
{
    private readonly OsDbContext _context = context;

    public async Task<IEnumerable<T>> GetListAsync<T>(BaseCondition? conditions) where T : BaseModel
    {
        var query = _context.Set<T>().AsQueryable();
        if (conditions == null)
        {
            return await query.ToListAsync();
        }
        else if (conditions is CompositeConditions compositeConditions)
        {
            var lambda = compositeConditions.ToLambda<T>();
            var response = await query.Where(lambda).ToListAsync();
            return response;
        }
        else if (conditions is SimpleCondition simpleCondition)
        {
            var composite = new CompositeConditions(LogicalSwitch.And, simpleCondition);

            var response = await query
                .Where(composite.ToLambda<T>())
                .ToListAsync();
            return response;
        }

        return await query.ToListAsync();
    }

    public async Task<T?> GetAsync<T>(Guid id) where T : BaseModel
    {
        var response = await _context.Set<T>().FindAsync(id);
        return response ?? null;
    }

    public async Task<T?> CreateAsync<T>(T entity) where T : BaseModel
    {
        var response = await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return response.Entity;
    }

    public async Task<T?> UpdateAsync<T>(T entity, Guid id) where T : BaseModel
    {
        var response = _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
        return response.Entity;
    }

    public async Task<bool> DeleteAsync<T>(Guid id) where T : BaseModel
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}