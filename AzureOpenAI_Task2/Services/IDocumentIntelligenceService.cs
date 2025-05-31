namespace AzureOpenAI_Task2.Services
{
    public interface IDocumentIntelligenceService
    {
        Task<string> ExtractTextAsync(IFormFile file);
    }
}
