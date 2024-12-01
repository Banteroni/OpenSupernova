﻿using OS.Data.Models;
using OS.Data.Repository.Conditions;
using System.Linq.Expressions;

namespace OS.Services.Repository;

public interface IRepository
{
    public IQueryable<T> GetQueryable<T>() where T : BaseModel;

    public Task<IEnumerable<T>> GetAllAsync<T>(string[]? modelsToInclude = null) where T : BaseModel;

    public Task<T?> GetAsync<T>(Guid id, string[]? modelsToInclude = null) where T : BaseModel;

    public Task<IEnumerable<T>> FindAllAsync<T>(Expression<Func<T, bool>> predicate, string[]? modelsToInclude = null) where T : BaseModel;

    public Task<T?> FindFirstAsync<T>(Expression<Func<T, bool>> predicate, string[]? modelsToInclude = null) where T : BaseModel;

    public Task<T?> DeleteAsync<T>(Guid id, bool saveChanges = true) where T : BaseModel;

    public Task<IEnumerable<T>> DeleteWhereAsync<T>(Expression<Func<T, bool>> predicate, bool saveChanges = true) where T : BaseModel;

    public Task<T> UpdateAsync<T>(T entity, bool saveChanges = true) where T : BaseModel;

    public Task<T> CreateAsync<T>(T entity, bool saveChanges = true) where T : BaseModel;

    public Task<bool> SaveChangesAsync();
}