using System.ComponentModel.DataAnnotations;

namespace AzureOpenAI_Task2.Models.ViewModels
{
    public class ChatViewModel
    {
        public int Id { get; set; }
        public string DocumentPath { get; set; }
        public string DocumentContent { get; set; }
        public List<MessageViewModel> Messages { get; set; }

    }
}
