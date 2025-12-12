using Microsoft.AspNetCore.Mvc;
using ZavaStorefront.Services;

namespace ZavaStorefront.Controllers
{
    public class ChatController : Controller
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IChatService _chatService;

        public ChatController(ILogger<ChatController> logger, IChatService chatService)
        {
            _logger = logger;
            _chatService = chatService;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Loading chat page");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Message))
            {
                return BadRequest(new { error = "Message cannot be empty" });
            }

            try
            {
                _logger.LogInformation("Processing chat message from user");
                var response = await _chatService.SendMessageAsync(request.Message);
                
                return Ok(new { response = response });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error processing chat message");
                return StatusCode(500, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in chat processing");
                return StatusCode(500, new { error = "An unexpected error occurred" });
            }
        }
    }

    public class ChatMessageRequest
    {
        public string? Message { get; set; }
    }
}
