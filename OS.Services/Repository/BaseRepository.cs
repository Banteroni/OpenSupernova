using OS.Data.Models;
using OS.Services.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OS.Services.Repository
{
    public abstract class BaseRepository : IRepository
    {
        private IDtoMapper _mapper;

        public BaseRepository(IDtoMapper mapper)
        {
            _mapper = mapper;
        }
        public abstract Task<T> CreateAsync<T>(T entity, bool saveChanges = true) where T : BaseModel;
        public abstract Task<T?> DeleteAsync<T>(Guid id, bool saveChanges = true) where T : BaseModel;

        public abstract Task<IEnumerable<T>> DeleteWhereAsync<T>(Expression<Func<T, bool>> predicate, bool saveChanges = true) where T : BaseModel;

        public abstract Task<IEnumerable<T>> FindAllAsync<T>(Expression<Func<T, bool>> predicate, string[]? modelsToInclude = null) where T : BaseModel;

        public async Task<IEnumerable<T2>> FindAllAsync<T, T2>(Expression<Func<T, bool>> predicate, string[]? modelsToInclude = null) where T : BaseModel
        {
            var results = await FindAllAsync<T>(predicate, modelsToInclude);
            return results.Select(result => _mapper.Map<T2>(result));
        }

        public abstract Task<T?> FindFirstAsync<T>(Expression<Func<T, bool>> predicate, string[]? modelsToInclude = null) where T : BaseModel;

        public async Task<T2?> FindFirstAsync<T, T2>(Expression<Func<T, bool>> predicate, string[]? modelsToInclude = null) where T : BaseModel
        {
            var result = await FindFirstAsync(predicate, modelsToInclude);
            return result == null ? default(T2) : _mapper.Map<T2>(result);
        }

        public abstract Task<IEnumerable<T>> GetAllAsync<T>(string[]? modelsToInclude = null) where T : BaseModel;

        public async Task<IEnumerable<T2>> GetAllAsync<T, T2>(string[]? modelsToInclude = null) where T : BaseModel
        {
            var result = await GetAllAsync<T>(modelsToInclude);
            return result.Select(r => _mapper.Map<T2>(r));
        }

        public abstract Task<T?> GetAsync<T>(Guid id, string[]? modelsToInclude = null) where T : BaseModel;

        public async Task<T2?> GetAsync<T, T2>(Guid id, string[]? modelsToInclude = null) where T : BaseModel
        {
            var result = await GetAsync<T>(id, modelsToInclude);
            return result == null ? default(T2) : _mapper.Map<T2>(result);
        }

        public abstract IQueryable<T> GetQueryable<T>() where T : BaseModel;

        public abstract Task<bool> SaveChangesAsync();

        public abstract Task<T> UpdateAsync<T>(T entity, bool saveChanges = true) where T : BaseModel;
    }
}
