using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using System.Text.Json;
using QuizGameServer.Application.Contracts;
using QuizGameServer.Application.Configurations;

namespace QuizGameServer.Infrastructure.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GeminiService> _logger;
        private readonly string _apiKey;

        public GeminiService(HttpClient httpClient, ILogger<GeminiService> logger, IOptions<GeminiOptions> options)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = options.Value.ApiKey ?? throw new Exception("Gemini API key not configured");
        }

        public async Task<List<QuizQuestion>> FetchQuestionsAsync(string topic, string difficulty, int numberOfQuestions = 5, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(topic) || topic.Length > 500)
            {
                _logger.LogError("Invalid topic input: {Topic}, Difficulty: {Difficulty}, NumberOfQuestions: {NumberOfQuestions}", topic, difficulty, numberOfQuestions);
                throw new ArgumentException("Topic must be between 1 and 500 characters");
            }
            if (string.IsNullOrEmpty(difficulty))
            {
                _logger.LogError("Invalid difficulty input: {Topic}, Difficulty: {Difficulty}, NumberOfQuestions: {NumberOfQuestions}", topic, difficulty, numberOfQuestions);
                throw new ArgumentException("Difficulty must be provided");
            }

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

            var prompt = $@"
                Tạo một bộ câu đố gồm chính xác {numberOfQuestions} câu hỏi trắc nghiệm về chủ đề '{topic}', với độ khó là '{difficulty}'.

                Yêu cầu:
                - Mỗi câu hỏi phải có chính xác 4 lựa chọn trả lời.
                - Có một và chỉ một đáp án đúng cho mỗi câu.
                - Trả về kết quả dưới dạng một mảng JSON hợp lệ.

                Mỗi phần tử trong mảng JSON là một đối tượng có các khóa:
                - ""question"" (string): Nội dung câu hỏi.
                - ""options"" (string[]): Một mảng gồm 4 lựa chọn.
                - ""correctAnswer"" (string): Một chuỗi chính xác là một trong các phần tử trong ""options"".

                Ví dụ định dạng đầu ra:

                [
                    {{
                    ""question"": ""Thủ đô của Việt Nam là gì?"",
                    ""options"": [""Hà Nội"", ""Đà Nẵng"", ""TP. Hồ Chí Minh"", ""Hải Phòng""],
                    ""correctAnswer"": ""Hà Nội""
                    }}
                ]
                ";

            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            var requestJson = System.Text.Json.JsonSerializer.Serialize(requestBody);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Gemini API error: {Status} {Reason}. Input: Topic={Topic}, Difficulty={Difficulty}, NumberOfQuestions={NumberOfQuestions}", response.StatusCode, response.ReasonPhrase, topic, difficulty, numberOfQuestions);
                throw new Exception($"Gemini API error: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(responseContent);
            var text = doc.RootElement
                            .GetProperty("candidates")[0]
                            .GetProperty("content")
                            .GetProperty("parts")[0]
                            .GetProperty("text")
                            .GetString();
            if (string.IsNullOrEmpty(text))
            {
                _logger.LogError("Phản hồi từ Gemini không chứa dữ liệu văn bản. Input: Topic={Topic}, Difficulty={Difficulty}, NumberOfQuestions={NumberOfQuestions}", topic, difficulty, numberOfQuestions);
                throw new Exception("Phản hồi từ Gemini không chứa dữ liệu văn bản.");
            }

            // Loại bỏ code fence nếu có
            var fenceRegex = new Regex("^```(\\w*)?\\s*\\n?(.*?)\\n?\\s*```$", RegexOptions.Singleline);
            var match = fenceRegex.Match(text);
            if (match.Success && match.Groups.Count > 2)
                text = match.Groups[2].Value.Trim();

            var parsedData = JsonConvert.DeserializeObject<List<QuizQuestion>>(text);
            if (parsedData == null || parsedData.Any(q => string.IsNullOrEmpty(q.Question) || q.Options == null || q.Options.Count != 4 || string.IsNullOrEmpty(q.CorrectAnswer)))
            {
                _logger.LogError("Dữ liệu trả về từ AI không đúng định dạng. Input: Topic={Topic}, Difficulty={Difficulty}, NumberOfQuestions={NumberOfQuestions}, RawText={RawText}", topic, difficulty, numberOfQuestions, text);
                throw new Exception("Dữ liệu trả về từ AI không đúng định dạng.");
            }

            return parsedData.Take(numberOfQuestions).ToList();
        }
    }
}