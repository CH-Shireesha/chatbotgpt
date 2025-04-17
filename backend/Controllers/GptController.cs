using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GptController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GptController> _logger;

        public GptController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<GptController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            _logger.LogInformation("Received chat request");
            
            if (string.IsNullOrEmpty(request.Message))
            {
                return BadRequest("Message cannot be empty");
            }

            var apiKey = _configuration["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return StatusCode(500, "API key is not configured");
            }

            var client = _httpClientFactory.CreateClient();

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = request.Message }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.9,
                    topK = 40,
                    topP = 0.95,
                    maxOutputTokens = 1024,
                }
            };

            var jsonRequest = JsonSerializer.Serialize(requestBody);
            _logger.LogInformation($"Request body: {jsonRequest}");

            var content = new StringContent(
                jsonRequest,
                Encoding.UTF8,
                "application/json"
            );

            _logger.LogInformation("Sending request to Gemini API");
            var response = await client.PostAsync(
                $"https://generativelanguage.googleapis.com/v1/models/gemini-1.5-pro:generateContent?key={apiKey}",
                content
            );

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Gemini API error: {errorContent}");
                return StatusCode((int)response.StatusCode, errorContent);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Raw Gemini API response: {responseContent}");

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseContent, options);
                _logger.LogInformation($"Deserialized response: {JsonSerializer.Serialize(geminiResponse)}");

                if (geminiResponse?.Candidates == null || geminiResponse.Candidates.Length == 0)
                {
                    _logger.LogError("No candidates in Gemini API response");
                    return StatusCode(500, "No response from Gemini API");
                }

                var firstCandidate = geminiResponse.Candidates[0];
                if (firstCandidate?.Content?.Parts == null || firstCandidate.Content.Parts.Length == 0)
                {
                    _logger.LogError("No content parts in Gemini API response");
                    return StatusCode(500, "Invalid response format from Gemini API");
                }

                _logger.LogInformation("Successfully processed chat request");
                return Ok(new { response = firstCandidate.Content.Parts[0].Text });
            }
            catch (JsonException ex)
            {
                _logger.LogError($"Error deserializing response: {ex.Message}");
                return StatusCode(500, $"Error processing response: {ex.Message}");
            }
        }
    }

    public class ChatRequest
    {
        public string? Message { get; set; }
    }

    public class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public Candidate[]? Candidates { get; set; }
    }

    public class Candidate
    {
        [JsonPropertyName("content")]
        public Content? Content { get; set; }
    }

    public class Content
    {
        [JsonPropertyName("parts")]
        public Part[]? Parts { get; set; }
    }

    public class Part
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
} 