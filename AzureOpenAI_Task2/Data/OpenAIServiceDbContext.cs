using AzureOpenAI_Task2.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace AzureOpenAI_Task2.Data
{
    public class OpenAIServiceDbContext : DbContext
    {
        public OpenAIServiceDbContext(DbContextOptions<OpenAIServiceDbContext> options) : base(options)
        {
            
        }
        public DbSet<ChatMessage> Messages { get; set; }
        public DbSet<Chat> Chats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Chat>(entity =>
            {
                entity.HasKey(e => e.SessionId);
                entity.HasMany(e => e.Messages).WithOne(m => m.ChatSession)
                .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
