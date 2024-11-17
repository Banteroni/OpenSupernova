using OS.Data.Models;
using OS.Data.Repository.Conditions;

namespace OS.Services.Repository;

public class MockRepository : IRepository
{
    public Task<IEnumerable<T>> GetListAsync<T>(BaseCondition? condition = null) where T : BaseModel
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetAsync<T>(Guid id) where T : BaseModel
    {
        throw new NotImplementedException();
    }

    public Task<T?> CreateAsync<T>(T entity) where T : BaseModel
    {
        throw new NotImplementedException();
    }

    public Task<T?> UpdateAsync<T>(T entity, Guid id) where T : BaseModel
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync<T>(Guid id) where T : BaseModel
    {
        throw new NotImplementedException();
    }
}