using Microsoft.EntityFrameworkCore;
using OS.Data.Context;
using OS.Data.Models;
using OS.Data.Repository.Conditions;

namespace OS.Services.Repository;

public class SqlRepository(OsDbContext context) : BaseRepository, IRepository
{
    private readonly OsDbContext _context = context;

    public async Task<IEnumerable<T>> GetListAsync<T>(CompositeCondition condition, string[]? modelsToInclude = null) where T : BaseModel
    {
        var query = _context.Set<T>().AsQueryable();
        var lambda = condition.ToLambda<T>();
        var populatedQuery = query.Where(lambda);
        populatedQuery = ApplySkipAndTake(populatedQuery, condition);
        // include  mdodels
        if (modelsToInclude != null)
        {
            foreach (var model in modelsToInclude)
            {
                populatedQuery = populatedQuery.Include(model);
            }
        }
        var queryList = await populatedQuery.Where(lambda).ToListAsync();
        return queryList;
    }

    public async Task<IEnumerable<T>> GetListAsync<T>(SimpleCondition condition, string[]? modelsToInclude = null) where T : BaseModel
    {
        var compositeCondition = new CompositeCondition(LogicalSwitch.And);
        compositeCondition.AddCondition(condition);
        return await GetListAsync<T>(compositeCondition, modelsToInclude);
    }

    public async Task<IEnumerable<T>> GetListAsync<T>(string[]? modelsToInclude = null) where T : BaseModel
    {
        return await GetListAsync<T>(new CompositeCondition(LogicalSwitch.And), modelsToInclude);
    }

    public async Task<T?> GetAsync<T>(Guid id, string[]? entitiesToInclude = null) where T : BaseModel
    {
        var query = _context.Set<T>().AsQueryable();
        if (entitiesToInclude != null)
        {
            query = entitiesToInclude.Aggregate(query, (current, entity) => current.Include(entity));
        }

        var response = await query.FirstOrDefaultAsync(x => x.Id == id);
        return response;
    }

    public async Task<T> CreateAsync<T>(T entity) where T : BaseModel
    {
        var response = await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return response.Entity;
    }

    public async Task<T> UpdateAsync<T>(T entity, Guid id) where T : BaseModel
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