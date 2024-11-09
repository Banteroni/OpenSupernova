namespace OS.Services.IRepository;

public interface IRepository
{
    public Task<T> GetAsync<T>(Guid id);
    public Task<IEnumerable<T>> GetAllAsync<T>(Query query);
}