﻿using System.Linq.Expressions;
using Play.Common.Service.Entities;

namespace Play.Common.Service.Repositories;

public interface IRepository<T> where T : IEntity
{
    Task CreateAsync(T entity);
    Task<IReadOnlyCollection<T>> GetAllAsync();
    Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter);
    Task<T> GetAsync(Guid id);
    Task<T> GetAsync(Expression<Func<T, bool>> filter);
    Task DeleteAsync(Guid id);
    Task UpdateAsync(T entity);
}