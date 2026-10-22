namespace DotNetAIChatbot.Entities
{
    public class ChatMessageEntity
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }

        public string Role { get; set; } = "user";

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Conversation? Conversation { get; set; }
    }
}
