using Microsoft.AspNetCore.Mvc;
using QuizGameServer.Services;
using System.Threading.Tasks;
using QuizGameServer.Models; // Thêm namespace cho model

namespace QuizGameServer.Controllers
{
    [ApiController]
    [Route("api/quiz")]
    public class QuestionsController : ControllerBase
    {
        private readonly GeminiService _googleGenerativeAIService;

        public QuestionsController(GeminiService googleGenerativeAIService)
        {
            _googleGenerativeAIService = googleGenerativeAIService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateQuestions([FromBody] QuizRequest request)
        {
            var questions = await _googleGenerativeAIService.FetchQuestionsAsync(request.Topic, request.Difficulty);
            return Ok(questions);
        }
    }
}