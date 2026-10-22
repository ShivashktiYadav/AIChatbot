using DotNetAIChatbot.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetAIChatbot.Data
{
    public class ChatbotDbContext : DbContext
    {
        public ChatbotDbContext(DbContextOptions<ChatbotDbContext> options) : base(options)
        {
        }

        public DbSet<Conversation> Conversations { get; set; }

        public DbSet<ChatMessageEntity> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Conversation>()
                .Property(c => c.Title)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<ChatMessageEntity>()
                .Property(m => m.Role)
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<ChatMessageEntity>()
                .Property(m => m.Content)
                .IsRequired();

            modelBuilder.Entity<Conversation>()
                .HasMany(c => c.Messages)
                .WithOne(m => m.Conversation)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChatMessageEntity>()
                .HasIndex(m => new
                {
                    m.ConversationId,
                    m.CreatedAt
                });
        }
    }
}
