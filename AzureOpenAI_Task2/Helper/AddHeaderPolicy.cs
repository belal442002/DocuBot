using Azure.Core.Pipeline;
using Azure.Core;

namespace AzureOpenAI_Task2.Helper
{
    public class AddHeaderPolicy : HttpPipelinePolicy
    {
        private readonly string _headerName;
        private readonly string _headerValue;

        public AddHeaderPolicy(string headerName, string headerValue)
        {
            _headerName = headerName;
            _headerValue = headerValue;
        }

        public override void Process(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
        {
            message.Request.Headers.Add(_headerName, _headerValue);
            ProcessNext(message, pipeline);
        }

        public override async ValueTask ProcessAsync(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
        {
            message.Request.Headers.Add(_headerName, _headerValue);
            await ProcessNextAsync(message, pipeline);
        }
    }
}
