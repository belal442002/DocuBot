using AzureOpenAI_Task2.Repositories;

namespace AzureOpenAI_Task2.UOW
{
    public interface IUnitOfWork : IDisposable
    {
        // Repositories
        IChatRepository ChatRepository { get; }
        IChatMessageRepository ChatMessageRepository { get; }

        // Methods
        Task BeginTransactionAsync();
        Task<int> SaveChangesAsync();
        Task CommitAsync();
        Task RollBackAsync();
        Task<bool> CompleteAsync();
    }
}
