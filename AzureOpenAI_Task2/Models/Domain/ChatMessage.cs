using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AzureOpenAI_Task2.Models.Domain
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(ChatSession))]
        public int SessionId { get; set; }
        public string Role { get; set; }
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now;

        // Navigation Property

        public Chat ChatSession { get; set; }
    }
}
