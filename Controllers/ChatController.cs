using DotNetAIChatbot.Data;
using DotNetAIChatbot.Entities;
using DotNetAIChatbot.Models;
using DotNetAIChatbot.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetAIChatbot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly OllamaService _ollamaService;
        private readonly ChatbotDbContext _context;

        public ChatController(OllamaService ollamaService,ChatbotDbContext context)
        {
            _ollamaService = ollamaService;
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<ChatResponse>> Post([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new
                {
                    error = "Message is required."
                });
            }

            if (request.Message.Length > 2000)
            {
                return BadRequest(new
                {
                    error = "Message must not exceed 2000 characters."
                });
            }

            try
            {
                Conversation conversation;

                // Existing conversation किंवा नवीन conversation
                if (request.ConversationId.HasValue)
                {
                    conversation = await _context.Conversations.FirstOrDefaultAsync(c =>
                    c.Id == request.ConversationId.Value);

                    if (conversation == null)
                    {
                        return NotFound(new
                        {
                            error = "Conversation not found."
                        });
                    }
                }
                else
                {
                    conversation = new Conversation
                    {
                        Title = request.Message.Length > 100
                            ? request.Message.Substring(0, 100)
                            : request.Message,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.Conversations.Add(conversation);
                    await _context.SaveChangesAsync();
                }

                // Current message save करण्याआधीचे शेवटचे 19 messages.
                // Current message मिळून AI ला जास्तीत जास्त 20 मिळतील.
                var history = await _context.ChatMessages
                    .Where(m =>
                        m.ConversationId == conversation.Id)
                    .OrderByDescending(m => m.CreatedAt)
                    .ThenByDescending(m => m.Id)
                    .Take(19)
                    .Select(m => new ChatMessage
                    {
                        Role = m.Role,
                        Content = m.Content
                    })
                    .ToListAsync();

                // AI ला chronological order मध्ये history द्या.
                history.Reverse();

                // User message database मध्ये save करा.
                var userMessage = new ChatMessageEntity
                {
                    ConversationId = conversation.Id,
                    Role = "user",
                    Content = request.Message,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ChatMessages.Add(userMessage);
                conversation.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Existing OllamaService method वापरा.
                var reply = await _ollamaService.GetResponseAsync(
                    request.Message,
                    history);

                // AI response database मध्ये save करा.
                var assistantMessage = new ChatMessageEntity
                {
                    ConversationId = conversation.Id,
                    Role = "assistant",
                    Content = reply,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ChatMessages.Add(assistantMessage);
                conversation.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new ChatResponse
                {
                    ConversationId = conversation.Id,
                    Reply = reply
                });
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, new
                {
                    error = "Ollama service is unavailable."
                });
            }
            catch (TaskCanceledException)
            {
                return StatusCode(504, new
                {
                    error = "AI response timed out."
                });
            }
        }

        // एखाद्या conversation चे messages load करा.
        [HttpGet("{conversationId}/messages")]
        public async Task<IActionResult> GetMessages(
            int conversationId,
            int page = 1,
            int pageSize = 20)
        {
            if (page < 1)
                page = 1;

            pageSize = Math.Clamp(pageSize, 1, 20);

            var exists = await _context.Conversations
                .AnyAsync(c => c.Id == conversationId);

            if (!exists)
            {
                return NotFound(new
                {
                    error = "Conversation not found."
                });
            }

            var messages = await _context.ChatMessages
                .Where(m => m.ConversationId == conversationId)
                .OrderByDescending(m => m.CreatedAt)
                .ThenByDescending(m => m.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new
                {
                    m.Id,
                    m.Role,
                    m.Content,
                    m.CreatedAt
                })
                .ToListAsync();

            messages.Reverse();

            return Ok(messages);
        }

        // सर्व conversations ची यादी.
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var conversations = await _context.Conversations
                .OrderByDescending(c => c.UpdatedAt)
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                    c.CreatedAt,
                    c.UpdatedAt
                })
                .ToListAsync();

            return Ok(conversations);
        }
    }
}
