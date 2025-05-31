namespace AzureOpenAI_Task2.Services
{
    public interface IDocumentService
    {
        Task<string> ExtractTextAsync(IFormFile file);
    }
}
