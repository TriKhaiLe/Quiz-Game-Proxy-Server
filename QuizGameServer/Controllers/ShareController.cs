using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QuizGameServer.Application.Interfaces;
using System.Text;

namespace QuizGameServer.Controllers
{
    [ApiController]
    [Route("share")]
    public class ShareController : ControllerBase
    {
        private readonly IQuizSharingService _quizSharingService;
        private readonly IConfiguration _config;
        public ShareController(IQuizSharingService quizSharingService, IConfiguration config)
        {
            _quizSharingService = quizSharingService;
            _config = config;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShareHtml([FromRoute] string id, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(id, out var guid))
                return Content(Render404(), "text/html; charset=utf-8");
            var quiz = await _quizSharingService.GetSharedQuizAsync(guid, cancellationToken);
            if (quiz == null || quiz.QuizContent == null || quiz.QuizContent.Questions == null || quiz.QuizContent.Questions.Count == 0)
                return Content(Render404(), "text/html; charset=utf-8");

            var idx = quiz.CurrentQuestionIndex;
            if (idx < 0 || idx >= quiz.QuizContent.Questions.Count)
                idx = 0;
            var previewQuestion = quiz.QuizContent.Questions[idx];
            var topic = quiz.QuizContent.Topic;
            var frontendBaseUrl = _config["FrontendBaseUrl"] ?? "https://quiz-game-trivia-master.vercel.app";
            var backendBaseUrl = _config["BackendBaseUrl"] ?? "https://your-backend-api.com";
            var previewImage = _config["SocialPreviewImage"] ?? "https://your-domain.com/social-preview-image.png";
            var redirectUrl = $"{frontendBaseUrl}/#/quiz/{id}";
            var shareUrl = $"{backendBaseUrl}/share/{id}";

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset=\"utf-8\" />");
            sb.AppendLine($"  <title>Đố bạn kiến thức {topic}!</title>");
            sb.AppendLine("  <meta property=\"og:type\" content=\"website\" />");
            sb.AppendLine($"  <meta property=\"og:url\" content=\"{shareUrl}\" />");
            sb.AppendLine($"  <meta property=\"og:title\" content=\"{previewQuestion.Question}\" />");
            sb.AppendLine($"  <meta property=\"og:description\" content=\"Lựa chọn: {string.Join(", ", previewQuestion.Options)}\" />");
            sb.AppendLine($"  <meta property=\"og:image\" content=\"{previewImage}\" />");
            sb.AppendLine("  <meta property=\"twitter:card\" content=\"summary_large_image\" />");
            sb.AppendLine($"  <meta property=\"twitter:url\" content=\"{shareUrl}\" />");
            sb.AppendLine($"  <meta property=\"twitter:title\" content=\"{previewQuestion.Question}\" />");
            sb.AppendLine($"  <meta property=\"twitter:description\" content=\"Lựa chọn: {string.Join(", ", previewQuestion.Options)}\" />");
            sb.AppendLine($"  <meta property=\"twitter:image\" content=\"{previewImage}\" />");
            sb.AppendLine($"  <meta http-equiv=\"refresh\" content=\"0; url={redirectUrl}\" />");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("  <p>Đang chuyển hướng đến câu đố...</p>");
            sb.AppendLine($"  <h2>{previewQuestion.Question}</h2>");
            sb.AppendLine("  <ul>");
            foreach (var opt in previewQuestion.Options)
                sb.AppendLine($"    <li>{opt}</li>");
            sb.AppendLine("  </ul>");
            sb.AppendLine($"  <script type=\"text/javascript\">window.location.href = '{redirectUrl}';</script>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return Content(sb.ToString(), "text/html; charset=utf-8");
        }

        private string Render404()
        {
            return "<!DOCTYPE html><html><head><meta charset=\"utf-8\" /><title>404 Not Found</title></head><body><h1>404 - Quiz not found</h1></body></html>";
        }
    }
}
