using QuizGameServer.Models;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading.Tasks;
using QuizGameServer.Configurations;
using Microsoft.Extensions.Logging;
using DotnetGeminiSDK.Client.Interfaces;
using Newtonsoft.Json;

namespace QuizGameServer.Services
{
    public class GeminiService
    {
        private readonly IGeminiClient _geminiClient;
        private readonly ILogger<GeminiService> _logger;

        public GeminiService(IGeminiClient geminiClient, ILogger<GeminiService> logger)
        {
            _geminiClient = geminiClient;
            _logger = logger;
        }

        public async Task<List<Question>> FetchQuestionsAsync(string topic, double difficulty, int numberOfQuestions = 5)
        {
            // check topic không được rỗng và dưới 100 ký tự
            if (string.IsNullOrEmpty(topic) || topic.Length > 100)
            {
                throw new ArgumentException("Topic must be between 1 and 100 characters");
            }

            // difficulty phải nằm trong khoảng từ 1 đến 10
            if (difficulty < 1 || difficulty > 10)
            {
                throw new ArgumentException("Difficulty must be between 1 and 10");
            }

            var prompt = $@"Generate {numberOfQuestions} multiple choice questions about {topic} with difficulty level {difficulty} in Vietnamese. The questions should be short, easy to understand, and general knowledge.
                Format the response as a JSON array with each question having the following structure:
                {{
                    ""content"": ""question text"",
                    ""options"": [""option1"", ""option2"", ""option3"", ""option4""],
                    ""correctOptionIndex"": 0-3
                    ""explanation"": ""A detailed explanation of why the correct answer is correct.""
                }}
                Make sure the questions are challenging but clear, and the options are plausible but with only one correct answer.
                IMPORTANT: Return ONLY the JSON array, no additional text.";

            var response = await _geminiClient.TextPrompt(prompt) ?? throw new Exception("Failed to generate quiz questions");
            try
            {
                var jsonText = response.Candidates.FirstOrDefault()?.Content.Parts.FirstOrDefault()?.Text;
                _logger.LogInformation("✅ Received JSON: {JsonText}", jsonText);

                if (string.IsNullOrEmpty(jsonText))
                {
                    throw new Exception("Empty response from the AI service");
                }

                if (!jsonText.Trim().EndsWith("]"))
                {
                    _logger.LogError("❌ Response may be truncated! Incomplete JSON: {JsonText}", jsonText);
                    return new List<Question>();
                }

                var result = JsonConvert.DeserializeObject<List<Question>>(jsonText);
                return result ?? [];
            }
            catch (JsonException ex)
            {
                throw new Exception($"Failed to parse generated questions: {ex.Message}. Response: {response.ToString()}");
            }
        }
    }
}