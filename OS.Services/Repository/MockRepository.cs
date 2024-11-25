using OS.Data.Models;
using OS.Data.Repository.Conditions;

namespace OS.Services.Repository;

public class MockRepository : BaseRepository, IRepository
{
    public List<Album> Albums { get; set; } = [];
    public List<Artist> Artists { get; set; } = [];
    public List<Track> Tracks { get; set; } = [];

    public Task<IEnumerable<T>> GetListAsync<T>(CompositeCondition condition, string[]? modelsToInclude = null) where T : BaseModel
    {
        var query = _extractCorrectQueryable<T>();
        var lambda = condition.ToLambda<T>();
        var populatedQuery = query.Where(lambda);
        populatedQuery = ApplySkipAndTake(populatedQuery, condition);
        var response = populatedQuery.ToList();
        return Task.FromResult(response.AsEnumerable());
    }

    public Task<IEnumerable<T>> GetListAsync<T>(SimpleCondition condition, string[]? modelsToInclude = null) where T : BaseModel
    {
        var compositeCondition = new CompositeCondition(LogicalSwitch.And);
        compositeCondition.AddCondition(condition);

        return GetListAsync<T>(compositeCondition);
    }

    public async Task<IEnumerable<T>> GetListAsync<T>(string[]? modelsToInclude = null) where T : BaseModel
    {
        return await GetListAsync<T>(new CompositeCondition(LogicalSwitch.And));
    }

    public Task<T?> GetAsync<T>(Guid id, string[]? entitiesToInclude = null) where T : BaseModel
    {
        var query = _extractCorrectQueryable<T>();
        var response = query.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(response);
    }

    public Task<T> CreateAsync<T>(T entity, bool saveNow = true) where T : BaseModel
    {
        var entities = _extractCorrectList<T>();
        entities.Add(entity);
        return Task.FromResult(entity);
    }

    public Task<T?> UpdateAsync<T>(T entity, Guid id, bool saveNow = true) where T : BaseModel
    {
        var entities = _extractCorrectList<T>();
        var entityToUpdate = entities.FirstOrDefault(x => x.Id == id);
        if (entityToUpdate == null)
        {
            return Task.FromResult<T?>(null);
        }

        entityToUpdate = entity;
        return Task.FromResult(entityToUpdate);
    }

    public Task<bool> DeleteAsync<T>(Guid id, bool saveNow = true) where T : BaseModel
    {
        var entities = _extractCorrectList<T>();
        var entityToDelete = entities.FirstOrDefault(x => x.Id == id);
        if (entityToDelete == null)
        {
            return Task.FromResult(false);
        }

        entities.Remove(entityToDelete);
        return Task.FromResult(true);
    }


    private List<T> _extractCorrectList<T>() where T : BaseModel
    {
        if (typeof(T) == typeof(Album))
        {
            return Albums.Cast<T>().ToList();
        }
        else if (typeof(T) == typeof(Artist))
        {
            return Artists.Cast<T>().ToList();
        }
        else if (typeof(T) == typeof(Track))
        {
            return Tracks.Cast<T>().ToList();
        }

        // Default case: return an empty list or handle unknown types if necessary
        return new List<T>();
    }


    private IQueryable<T> _extractCorrectQueryable<T>() where T : BaseModel
    {
        var entities = _extractCorrectList<T>();
        return entities.AsQueryable();
    }

    public Task SaveChangesAsync()
    {
        return Task.CompletedTask;
    }
}