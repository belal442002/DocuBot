using AzureOpenAI_Task2.APIRespnses;
using AzureOpenAI_Task2.Models.Domain;
using AzureOpenAI_Task2.Models.ViewModels;

namespace AzureOpenAI_Task2.MappingProfile
{
    public static class ChatMessageProfileMapping
    {
        public static MessageViewModel ToMessageViewModel(this ChatMessage message)
        {
            return new MessageViewModel
            {
                Id = message.Id,
                Content = message.Content,
                Role = message.Role,
            };
        }

        public static Message ToMessage(this ChatMessage message)
        {
            return new Message
            {
                Content = message.Content,
                Role = message.Role,
            };
        }


        public static List<MessageViewModel> ToMessageViewModelList(this ICollection<ChatMessage> messages)
        {
            var messagesList = new List<MessageViewModel>();

            foreach (var message in messages)
            {
                messagesList.Add(message.ToMessageViewModel());
            }

            return messagesList;
        }

        public static List<Message> ToMessageList(this ICollection<ChatMessage> messages)
        {
            var messagesList = new List<Message>();

            foreach (var message in messages)
            {
                messagesList.Add(message.ToMessage());
            }

            return messagesList;
        }

        public static ChatMessage ToMessageModel(this Message message, int sessionId)
        {
            return new ChatMessage
            {
                Role = message.Role,
                Content = message.Content,
                SessionId = sessionId
            };
        }

    }
}
