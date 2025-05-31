namespace AzureOpenAI_Task2.APIRespnses
{
    public class ChatCompletionResponse
    {
        public string Id { get; set; }
        public long Created { get; set; }
        public List<Choice> Choices { get; set; }
        public Usage Usage { get; set; }
    }

    public class Choice
    {
        public int Index { get; set; }
        public string FinishReason { get; set; }
        public Message Message { get; set; }
    }

    public class Message
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }

    public class Usage
    {
        public int CompletionTokens { get; set; }
        public int PromptTokens { get; set; }
        public int TotalTokens { get; set; }
    }
}
