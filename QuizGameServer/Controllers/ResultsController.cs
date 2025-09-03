using Microsoft.AspNetCore.Mvc;
using QuizGameServer.Application.Contracts;
using QuizGameServer.Application.Interfaces;

namespace QuizGameServer.Controllers
{
    [ApiController]
    [Route("api/results")]
    public class ResultsController : ControllerBase
    {
        private readonly IQuizResultSharingService _service;
        public ResultsController(IQuizResultSharingService service)
        {
            _service = service;
        }

        [HttpPost("share")]
        public async Task<IActionResult> ShareResult([FromBody] ShareQuizResultDto request, CancellationToken cancellationToken)
        {
            if (request == null 
                || string.IsNullOrWhiteSpace(request.Topic) 
                || request.UserAnswers == null 
                || request.UserAnswers.Count == 0 
                || request.Questions == null 
                || request.Questions.Count == 0)
                return BadRequest();
            var id = await _service.ShareQuizResultAsync(request.Topic, request.Difficulty, request.UserAnswers, request.Questions, cancellationToken);
            return Created(string.Empty, new { id = id.ToString() });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetResult([FromRoute] string id, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(id, out var guid))
                return NotFound(new { error = "Quiz result not found." });
            var result = await _service.GetQuizResultAsync(guid, cancellationToken);
            if (result == null)
                return NotFound(new { error = "Quiz result not found." });
            return Ok(new ShareQuizResultDto
            {
                Topic = result.Topic,
                Difficulty = result.Difficulty,
                UserAnswers = result.UserAnswers,
                Questions = result.QuizContent?.Questions.Select(q => new SharedQuizQuestionDto
                {
                    Question = q.Question,
                    Options = q.Options,
                    CorrectAnswer = q.CorrectAnswer
                }).ToList() ?? []
            });
        }
    }
}
