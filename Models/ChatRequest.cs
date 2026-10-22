namespace DotNetAIChatbot.Models
{
    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;

        public List<ChatMessage> History { get; set; } = new();

        public int? ConversationId { get; set; }
    }
}
