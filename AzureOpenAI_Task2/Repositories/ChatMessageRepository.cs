using AzureOpenAI_Task2.Data;
using AzureOpenAI_Task2.Models.Domain;

namespace AzureOpenAI_Task2.Repositories
{
    public class ChatMessageRepository : GenericRepository<ChatMessage> , IChatMessageRepository
    {
        private readonly OpenAIServiceDbContext _dbContext;

        public ChatMessageRepository(OpenAIServiceDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
