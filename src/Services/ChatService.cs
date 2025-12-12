using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ZavaStorefront.Services
{
    public interface IChatService
    {
        Task<string> SendMessageAsync(string userMessage);
    }

    public class ChatService : IChatService
    {
        private readonly ILogger<ChatService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public ChatService(ILogger<ChatService> logger, IConfiguration configuration, HttpClient httpClient)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<string> SendMessageAsync(string userMessage)
        {
            try
            {
                _logger.LogInformation("Sending message to Phi4 endpoint: {Message}", userMessage);

                var endpointUrl = _configuration["Phi4Endpoint:Url"];
                var apiKey = _configuration["Phi4Endpoint:ApiKey"];

                if (string.IsNullOrEmpty(endpointUrl))
                {
                    _logger.LogError("Phi4Endpoint:Url is not configured");
                    throw new InvalidOperationException("Phi4 endpoint URL is not configured");
                }

                // Prepare the request payload
                var requestPayload = new
                {
                    message = userMessage,
                    max_tokens = 500
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(requestPayload),
                    Encoding.UTF8,
                    "application/json"
                );

                // Add authorization header if API key is provided
                if (!string.IsNullOrEmpty(apiKey))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", apiKey);
                }

                // Send request to Phi4 endpoint
                var response = await _httpClient.PostAsync(endpointUrl, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received response from Phi4 endpoint");

                // Parse the response
                using JsonDocument doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;

                // Extract the response text (adjust based on actual endpoint response format)
                string result = root.TryGetProperty("response", out var responseProp)
                    ? responseProp.GetString() ?? "No response from endpoint"
                    : root.TryGetProperty("text", out var textProp)
                        ? textProp.GetString() ?? "No response from endpoint"
                        : responseContent;

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error communicating with Phi4 endpoint");
                throw new InvalidOperationException($"Failed to communicate with Phi4 endpoint: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error parsing Phi4 endpoint response");
                throw new InvalidOperationException($"Failed to parse endpoint response: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in ChatService");
                throw;
            }
        }
    }
}
