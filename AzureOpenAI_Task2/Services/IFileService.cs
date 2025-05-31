namespace AzureOpenAI_Task2.Services
{
    public interface IFileService
    {
        Task<string?> SaveFileAsync(IFormFile file);
        bool RemoveFile(string filePath);
    }
}
