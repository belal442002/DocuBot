using AzureOpenAI_Task2.APIRespnses;

namespace AzureOpenAI_Task2.Services
{
    public interface IOpenAIService
    {
        Task<Message> GetChatCompletionAsync(List<Message> messages);
        Task<Message> GetChatCompletionUsingSDKAsync(List<Message> messages);
    }
}
