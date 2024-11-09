namespace OS.Services.IRepository;

public class SqlRepository : IRepository
{
    public Task<T> GetAsync<T>(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> GetAllAsync<T>(Query query)
    {
        throw new NotImplementedException();
    }
}