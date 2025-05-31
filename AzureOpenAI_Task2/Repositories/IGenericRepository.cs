﻿using System.Linq.Expressions;

namespace AzureOpenAI_Task2.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync<TId>(TId id);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>>? filter = null,
                                    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                    Func<IQueryable<T>, IQueryable<T>>? include = null);

        Task<List<T>> GetAsync(Expression<Func<T, bool>>? filter = null,
                                         Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                         Func<IQueryable<T>, IQueryable<T>>? include = null);

        void Update(T entity);
        void Delete(T entity);
        
    }
}
