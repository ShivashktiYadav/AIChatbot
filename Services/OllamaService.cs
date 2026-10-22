using DotNetAIChatbot.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAIChatbot.Services
{
    public class OllamaService
    {
        private readonly HttpClient _httpClient;

        public OllamaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
       
        public async Task<string> GetResponseAsync( string message,List<ChatMessage>? history = null)
        {
            // Keep only the last 20 valid conversation messages.
            var messages = (history ?? new List<ChatMessage>())
                .Where(m =>
                    (m.Role == "user" || m.Role == "assistant") &&
                    !string.IsNullOrWhiteSpace(m.Content))
                .TakeLast(20)
                .Select(m => new
                {
                    role = m.Role,
                    content = m.Content
                })
                .ToList();

            // Add the current question after the previous history.
            messages.Add(new
            {
                role = "user",
                content = message
            });

            var request = new
            {
                model = "llama3.2:1b",
                messages = messages,
                stream = false
            };

            using var response = await _httpClient.PostAsJsonAsync(
                "api/chat",
                request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content
                .ReadFromJsonAsync<OllamaChatResponse>();

            return result?.Message?.Content
                ?? "AI कडून उत्तर मिळाले नाही.";
        }

        private class OllamaChatResponse
        {
            public OllamaMessage? Message { get; set; }
        }

        private class OllamaMessage
        {
            public string Content { get; set; } = string.Empty;
        }
    }
}
