using System.ComponentModel.DataAnnotations;

namespace AzureOpenAI_Task2.Models.Domain
{
    public class Chat
    {
        [Key]
        public int SessionId { get; set; }
        public string DocumentPath { get; set; }
        public string DocumentContent { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<ChatMessage> Messages { get; set; }

    }
}
