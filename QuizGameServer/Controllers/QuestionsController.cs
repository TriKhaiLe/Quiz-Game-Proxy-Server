using Microsoft.AspNetCore.Mvc;
using QuizGameServer.Services;
using System.Threading.Tasks;

namespace QuizGameServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly GoogleGenerativeAIService _googleGenerativeAIService;

        public QuestionsController(GoogleGenerativeAIService googleGenerativeAIService)
        {
            _googleGenerativeAIService = googleGenerativeAIService;
        }

        [HttpGet]
        public async Task<IActionResult> GetQuestions(string topic, string difficulty)
        {
            var questions = await _googleGenerativeAIService.FetchQuestionsAsync(topic, difficulty);
            return Ok(questions);
        }
    }
}