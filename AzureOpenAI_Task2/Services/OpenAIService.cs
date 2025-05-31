using Azure.AI.OpenAI;
using AzureOpenAI_Task2.APIRespnses;
using AzureOpenAI_Task2.Helper;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Text;
using System.Text.Json;

namespace AzureOpenAI_Task2.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _endpoint;
        private readonly string _deploymentName;
        private readonly string _location;

        public OpenAIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            var AzureOpenAIConfig = _configuration.GetSection("AzureOpenAI");
            _apiKey = AzureOpenAIConfig["Key"]!;
            _endpoint = AzureOpenAIConfig["Api"]!;
            _deploymentName = AzureOpenAIConfig["DeploymentName"]!;
            _location = AzureOpenAIConfig["Location"]!;
        }
        public async Task<Message> GetChatCompletionAsync(List<Message> messages)
        {
            var requestBody = new
            {
                messages = messages
            };

            var requestUrl = $"{_endpoint}/openai/deployments/{_deploymentName}/chat/completions?api-version=2024-10-21";

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            requestMessage.Headers.Add("api-key", _apiKey);
            requestMessage.Headers.Add("Location", _location);
            requestMessage.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<ChatCompletionResponse>(responseContent);
            if(responseObject != null)
            {
                return responseObject.Choices[0].Message;
            }
            throw new Exception("Response has no content");
        }

        public async Task<Message> GetChatCompletionUsingSDKAsync(List<Message> messages)
        {

            AzureOpenAIClient openAIClient = new(
               new Uri(_endpoint),
               new ApiKeyCredential(_apiKey));

            ChatClient chatClient = openAIClient.GetChatClient(_deploymentName);

            var chatCompletionMessages = new OpenAI.Chat.ChatMessage[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                if (messages[i].Role.Equals(Roles.system.ToString()))
                {
                    chatCompletionMessages[i] = new SystemChatMessage(messages[i].Content);
                }
                else if (messages[i].Role.Equals(Roles.user.ToString()))
                {
                    chatCompletionMessages[i] = new UserChatMessage(messages[i].Content);
                }
                else if (messages[i].Role.Equals(Roles.assistant.ToString()))
                {
                    chatCompletionMessages[i] = new AssistantChatMessage(messages[i].Content);
                }
            }
            ChatCompletion completion = await chatClient.CompleteChatAsync(chatCompletionMessages);

            return new AzureOpenAI_Task2.APIRespnses.Message
            {
                Role = Roles.system.ToString(),
                Content = completion.Content[0].Text
            };
        }
    }
}
