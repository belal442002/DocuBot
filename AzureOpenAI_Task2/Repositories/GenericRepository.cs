﻿
using AzureOpenAI_Task2.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AzureOpenAI_Task2.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly OpenAIServiceDbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(OpenAIServiceDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }
        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public async Task AddRangeAsync(IEnumerable<T> entities) => await _dbSet.AddRangeAsync(entities);   
        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task<T?> GetByIdAsync<TId>(TId id) => await _dbSet.FindAsync(id);
        public void Update(T entity) => _dbSet.Update(entity);
        public void Delete(T entity) => _dbSet.Remove(entity);
        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>>? filter = null,
                                    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                    Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAsync(Expression<Func<T, bool>>? filter = null,
                                                      Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                                      Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            if(orderBy != null)
                query = orderBy(query);

            if(include != null)
                query = include(query);

            return await query.ToListAsync();
        }
        
    }
}
