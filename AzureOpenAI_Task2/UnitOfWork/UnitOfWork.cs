using AzureOpenAI_Task2.Data;
using AzureOpenAI_Task2.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace AzureOpenAI_Task2.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OpenAIServiceDbContext _dbContext;
        private IDbContextTransaction? _transaction;

        public IChatRepository ChatRepository { get; private set; }
        public IChatMessageRepository ChatMessageRepository { get; private set; }

        public UnitOfWork(OpenAIServiceDbContext dbContext)
        {
            _dbContext = dbContext;
            _transaction = null;

            ChatMessageRepository = new ChatMessageRepository(dbContext);
            ChatRepository = new ChatRepository(dbContext);
        }
        public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();
        

        public async Task BeginTransactionAsync()
        {
            if(_transaction != null) 
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }
            _transaction = await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction is null) 
            {
                throw new InvalidOperationException("No transaction to commit.");
            }
            await _transaction.CommitAsync();
            _transaction = null;
        }

        public async Task RollBackAsync()
        {
            if(_transaction is null)
            {
                throw new InvalidOperationException("No transaction to roll back.");
            }
            await _transaction.RollbackAsync();
            _transaction = null;
        }

        public async Task<bool> CompleteAsync()
        {
            try
            {
                await SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
                return true;
            }
            catch (Exception)
            {
                if (_transaction != null)
                {
                    await _transaction.RollbackAsync();
                }
                return false;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _dbContext.Dispose();
        }
    }
}
