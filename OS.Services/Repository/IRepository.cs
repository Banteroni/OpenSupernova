using OS.Data.Models;
using OS.Data.Repository.Conditions;

namespace OS.Services.Repository;

public interface IRepository
{
    public Task<IEnumerable<T>> GetListAsync<T>(CompositeCondition condition, string[]? modelsToInclude = null) where T : BaseModel;

    public Task<IEnumerable<T>> GetListAsync<T>(SimpleCondition condition, string[]? modelsToInclude = null) where T : BaseModel;

    public Task<IEnumerable<T>> GetListAsync<T>(string[]? modelsToInclude = null) where T : BaseModel;

    public Task<T?> GetAsync<T>(Guid id, string[]? entitiesToInclude = null) where T : BaseModel;

    public Task<T> CreateAsync<T>(T entity) where T : BaseModel;

    public Task<T> UpdateAsync<T>(T entity, Guid id) where T : BaseModel;

    public Task<bool> DeleteAsync<T>(Guid id) where T : BaseModel;
}