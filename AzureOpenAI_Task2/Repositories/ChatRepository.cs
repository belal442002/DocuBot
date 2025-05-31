using AzureOpenAI_Task2.Data;
using AzureOpenAI_Task2.Models.Domain;

namespace AzureOpenAI_Task2.Repositories
{
    public class ChatRepository : GenericRepository<Chat>, IChatRepository 
    {
        private readonly OpenAIServiceDbContext dbContext;

        public ChatRepository(OpenAIServiceDbContext _dbContext) : base(_dbContext)
        {
            dbContext = _dbContext;
        }
    }
}
