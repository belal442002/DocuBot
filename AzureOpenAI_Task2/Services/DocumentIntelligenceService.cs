using System.Text.Json;

namespace AzureOpenAI_Task2.Services
{
    public class DocumentIntelligenceService : IDocumentIntelligenceService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _endpoint;

        public DocumentIntelligenceService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            var azureDocumentIntelligenceConfig = _configuration.GetSection("AzureDocumentIntelligence");
            _apiKey = azureDocumentIntelligenceConfig["ApiKey"]!;
            _endpoint = azureDocumentIntelligenceConfig["Endpoint"]!;
        }

        private async Task<string> AnalyzeDocumentAsync(byte[] fileBytes)
        {
            try
            {
                var requestUrl = $"{_endpoint}/documentintelligence/documentModels/prebuilt-read:analyze?_overload=analyzeDocument&api-version=2024-11-30";

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl);
                requestMessage.Headers.Add("Ocp-Apim-Subscription-Key", _apiKey);
                requestMessage.Content = new ByteArrayContent(fileBytes);
                requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                var response = await _httpClient.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();

                if (response.Headers.TryGetValues("Operation-Location", out var operationLocationValues))
                {
                    return operationLocationValues.FirstOrDefault()!;
                }
                throw new Exception("Operation-Location header not found in the response.");
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<string> GetAnalyzeResultAsync(string operationLocation)
        {
            while(true)
            {
                var response = await _httpClient.GetAsync(operationLocation);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                if(result.Contains("\"status\":\"succeeded\""))
                {
                    return result;
                }
                await Task.Delay(3000);
            }
        }

        public async Task<string> ExtractTextAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new Exception("No file uploaded.");
                }

                // Read file into byte array
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                var operationLocation = await AnalyzeDocumentAsync(fileBytes);
                var result = await GetAnalyzeResultAsync(operationLocation);

                var json = JsonSerializer.Deserialize<JsonElement>(result);

                if(json.TryGetProperty("analyzeResult", out var analyzeResultValue)
                    &&analyzeResultValue.TryGetProperty("content", out var contentValue))
                {
                    return contentValue.GetString()!;
                }
                throw new Exception("No text to extract");

            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
