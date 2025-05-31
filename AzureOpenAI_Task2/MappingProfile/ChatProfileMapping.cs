using AzureOpenAI_Task2.Models.Domain;
using AzureOpenAI_Task2.Models.ViewModels;

namespace AzureOpenAI_Task2.MappingProfile
{
    public static class ChatProfileMapping
    {
        public static ChatViewModel ToChatViewModel(this Chat chatModel)
        {
            var messagesList = new List<MessageViewModel>();
            if (chatModel.Messages != null)
            {
                messagesList = chatModel.Messages.Count != 0 ?
                ChatMessageProfileMapping.ToMessageViewModelList(chatModel.Messages) : null;
            }    

            return new ChatViewModel
            {
                Id = chatModel.SessionId,
                DocumentPath = chatModel.DocumentPath,
                DocumentContent = chatModel.DocumentContent,
                Messages = messagesList
            };
        }
    }
}
