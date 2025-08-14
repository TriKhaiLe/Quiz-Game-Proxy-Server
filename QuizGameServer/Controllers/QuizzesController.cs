using Microsoft.AspNetCore.Mvc;
using QuizGameServer.Application.Contracts;
using QuizGameServer.Application.Interfaces;

namespace QuizGameServer.Controllers
{
    [ApiController]
    [Route("api/quizzes")]
    public class QuizzesController : ControllerBase
    {
        private readonly IQuizSharingService _quizSharingService;
        public QuizzesController(IQuizSharingService quizSharingService)
        {
            _quizSharingService = quizSharingService;
        }

        // POST /api/quizzes/share
        [HttpPost("share")]
        public async Task<IActionResult> ShareQuiz([FromBody] ShareQuizRequest request, CancellationToken cancellationToken)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Topic) || request.Questions == null || request.Questions.Count == 0)
                return BadRequest();
            var id = await _quizSharingService.ShareQuizAsync(request.Topic, request.Difficulty, request.CurrentQuestionIndex, request.Questions, cancellationToken);
            return Created(string.Empty, new { id = id.ToString() });
        }

        // GET /api/quizzes/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSharedQuiz([FromRoute] string id, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(id, out var guid))
                return NotFound(new { error = "Quiz not found." });
            var quiz = await _quizSharingService.GetSharedQuizAsync(guid, cancellationToken);
            if (quiz == null || quiz.QuizContent == null)
                return NotFound(new { error = "Quiz not found." });
            return Ok(new GetSharedQuizResponse
            {
                Topic = quiz.QuizContent.Topic,
                Difficulty = quiz.QuizContent.Difficulty,
                StartIndex = quiz.CurrentQuestionIndex,
                Questions = quiz.QuizContent.Questions.Select(q => new SharedQuizQuestionDto
                {
                    Question = q.Question,
                    Options = q.Options,
                    CorrectAnswer = q.CorrectAnswer
                }).ToList()
            });
        }
    }
}
