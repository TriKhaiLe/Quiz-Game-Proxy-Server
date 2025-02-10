using QuizGameServer.Models;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using QuizGameServer.Configurations;
using Microsoft.Extensions.Logging;

namespace QuizGameServer.Services
{
    public class GoogleGenerativeAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _apiKey;
        private readonly ILogger<GoogleGenerativeAIService> _logger;

        public GoogleGenerativeAIService(HttpClient httpClient, IOptions<GoogleGenerativeAIOptions> options, ILogger<GoogleGenerativeAIService> logger)
        {
            _httpClient = httpClient;
            _apiKey = options.Value.ApiKey;
            _logger = logger;
        }

        public async Task<List<Question>> FetchQuestionsAsync(string topic, string difficulty)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogError("API key is missing");
                throw new InvalidOperationException("API key is missing");
            }

            var prompt = $"Create 5 multiple-choice questions on the topic \"{topic}\" with difficulty level {difficulty} in Vietnamese. Each question has 4 possible answers, one of which is correct. The questions should be short, easy to understand, and general knowledge. Format the response as a JSON array: [{{ \"question\": \"text\", \"answers\": [\"a\", \"b\", \"c\", \"d\"], \"correctAnswer\": \"answer\" }}]";
            var response = await _httpClient.PostAsync($"https://api.google.com/generative-ai?apiKey={_apiKey}", new StringContent(prompt));
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(jsonResponse))
            {
                _logger.LogError("Empty response from the AI service");
                throw new InvalidOperationException("Empty response from the AI service");
            }
            
            return JsonConvert.DeserializeObject<List<Question>>(jsonResponse);
        }
    }
}