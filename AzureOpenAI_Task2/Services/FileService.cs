
namespace AzureOpenAI_Task2.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public bool RemoveFile(string filePath)
        {
            try
            {
                var physicalFileLocation = Path.Combine(_env.ContentRootPath, "wwwroot", filePath);
                if (File.Exists(physicalFileLocation))
                {
                    File.Delete(physicalFileLocation);
                    return true;
                }

                return false;

            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string?> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var folderName = "files";
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";

            var uploadsFolder = Path.Combine(_env.WebRootPath, folderName);
            if(!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using(var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return Path.Combine(folderName, uniqueFileName).Replace("\\", "/");
        }
    }
}
