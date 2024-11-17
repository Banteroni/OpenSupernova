using OS.Data.Models;
using OS.Data.Repository.Conditions;

namespace OS.Services.Repository;

public interface IRepository
{
    public Task<IEnumerable<T>> GetListAsync<T>(BaseCondition? condition = null) where T : BaseModel;
    
    public Task<T?> GetAsync<T>(Guid id) where T : BaseModel;
    
    public Task<T?> CreateAsync<T>(T entity) where T : BaseModel;
    
    public Task<T?> UpdateAsync<T>(T entity, Guid id) where T : BaseModel;
    
    public Task<bool> DeleteAsync<T>(Guid id) where T : BaseModel;
    
}