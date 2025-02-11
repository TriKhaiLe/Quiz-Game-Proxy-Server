using Microsoft.AspNetCore.Mvc;
using QuizGameServer.Services;
using System.Threading.Tasks;

namespace QuizGameServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly GeminiService _googleGenerativeAIService;

        public QuestionsController(GeminiService googleGenerativeAIService)
        {
            _googleGenerativeAIService = googleGenerativeAIService;
        }

        [HttpGet]
        public async Task<IActionResult> GetQuestions(string topic, double difficulty)
        {
            var questions = await _googleGenerativeAIService.FetchQuestionsAsync(topic, difficulty);
            return Ok(questions);
        }
    }
}