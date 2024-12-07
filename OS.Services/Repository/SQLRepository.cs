using Microsoft.EntityFrameworkCore;
using OS.Data.Context;
using OS.Data.Models;
using OS.Services.Mappers;
using System.Linq.Expressions;

namespace OS.Services.Repository;

public class SqlRepository(OsDbContext context, IDtoMapper mapper) : BaseRepository(mapper)
{
    private readonly OsDbContext _context = context;

    public override async Task<T> CreateAsync<T>(T entity, bool saveChanges = true)
    {
        await _context.Set<T>().AddAsync(entity);
        if (saveChanges)
        {
            await _context.SaveChangesAsync();
        }
        return entity;
    }

    public override async Task<T?> DeleteAsync<T>(Guid id, bool saveChanges = true) where T : class
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity == null)
        {
            return null;
        }
        _context.Set<T>().Remove(entity);
        if (saveChanges)
        {
            await _context.SaveChangesAsync();
        }
        return entity;
    }

    public override async Task<IEnumerable<T>> DeleteWhereAsync<T>(Expression<Func<T, bool>> predicate, bool saveChanges = true) where T : class
    {
        var entities = await GetQueryable<T>().Where(predicate).ToListAsync();
        if (entities.Count == 0)
        {
            return [];
        }
        _context.Set<T>().RemoveRange(entities);
        if (saveChanges)
        {
            await _context.SaveChangesAsync();
        }
        return entities;
    }

    public override async Task<IEnumerable<T>> FindAllAsync<T>(Expression<Func<T, bool>> predicate, string[]? modelsToInclude = null) where T : class
    {
        var query = GetQueryable<T>().Where(predicate);
        if (modelsToInclude != null)
        {
            foreach (var model in modelsToInclude)
            {
                query = query.Include(model);
            }
        }
        return await query.ToListAsync();
    }

    public override async Task<T?> FindFirstAsync<T>(Expression<Func<T, bool>> predicate, string[]? modelsToInclude = null) where T : class
    {
        var query = GetQueryable<T>().Where(predicate);
        if (modelsToInclude != null)
        {
            foreach (var model in modelsToInclude)
            {
                query = query.Include(model);
            }
        }
        return await query.FirstOrDefaultAsync();
    }

    public override async Task<IEnumerable<T>> GetAllAsync<T>(string[]? modelsToInclude = null) where T : class
    {
        var query = GetQueryable<T>();
        if (modelsToInclude != null)
        {
            foreach (var model in modelsToInclude)
            {
                query = query.Include(model);
            }
        }
        return await query.ToListAsync();
    }

    public override async Task<T?> GetAsync<T>(Guid id, string[]? modelsToInclude = null) where T : class
    {
        var query = GetQueryable<T>().Where(x => x.Id == id);
        if (modelsToInclude != null)
        {
            foreach (var model in modelsToInclude)
            {
                query = query.Include(model);
            }
        }
        return await query.FirstOrDefaultAsync();
    }

    public override IQueryable<T> GetQueryable<T>() where T : class
    {
        var query = _context.Set<T>().AsQueryable();
        return query;
    }

    public override async Task<bool> SaveChangesAsync()
    {
        var response = await _context.SaveChangesAsync();
        return response > 0;
    }

    public override async Task<T> UpdateAsync<T>(T entity, bool saveChanges = true) where T : class
    {
        _context.Set<T>().Update(entity);
        if (saveChanges)
        {
            await _context.SaveChangesAsync();
        }
        return entity;
    }
}