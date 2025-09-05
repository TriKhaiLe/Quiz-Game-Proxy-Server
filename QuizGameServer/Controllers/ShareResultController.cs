using Microsoft.AspNetCore.Mvc;
using QuizGameServer.Application.Interfaces;
using System.Text;

namespace QuizGameServer.Controllers
{
    [ApiController]
    [Route("share-result")]
    public class ShareResultController : ControllerBase
    {
        private readonly IQuizResultSharingService _service;
        private readonly IConfiguration _config;
        public ShareResultController(IQuizResultSharingService service, IConfiguration config)
        {
            _service = service;
            _config = config;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShareResultHtml([FromRoute] string id, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(id, out var guid))
                return Content(Render404(), "text/html; charset=utf-8");
            var result = await _service.GetQuizResultAsync(guid, cancellationToken);
            if (result == null || result.QuizContent == null || result.UserAnswers == null || result.UserAnswers.Count == 0)
                return Content(Render404(), "text/html; charset=utf-8");

            int score = 0;
            int total = result.QuizContent.Questions.Count;
            for (int i = 0; i < total && i < result.UserAnswers.Count; i++)
            {
                if (result.UserAnswers[i] == result.QuizContent.Questions[i].CorrectAnswer)
                    score++;
            }
            var topic = result.QuizContent.Topic;
            var frontendBaseUrl = _config["FrontendBaseUrl"] ?? "https://quiz-game-trivia-master.vercel.app";
            var backendBaseUrl = _config["BackendBaseUrl"] ?? "https://your-backend-api.com";
            var previewImage = _config["SocialPreviewImage"] ?? "https://your-domain.com/social-preview-image.png";
            var redirectUrl = $"{frontendBaseUrl}/#/result/{id}";
            var shareUrl = $"{backendBaseUrl}/share-result/{id}";

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset=\"utf-8\" />");
            sb.AppendLine($"  <title>Mình đã đạt {score}/{total} điểm!</title>");
            sb.AppendLine("  <meta property=\"og:type\" content=\"website\" />");
            sb.AppendLine($"  <meta property=\"og:url\" content=\"{shareUrl}\" />");
            sb.AppendLine($"  <meta property=\"og:title\" content=\"Mình đã đạt {score}/{total} điểm!\" />");
            sb.AppendLine($"  <meta property=\"og:description\" content=\"Bài trắc nghiệm về {topic}! Bạn có thể làm tốt hơn không. Nhấn để xem chi tiết và thử sức!\" />");
            sb.AppendLine($"  <meta property=\"og:image\" content=\"{previewImage}\" />");
            sb.AppendLine("  <meta property=\"twitter:card\" content=\"summary_large_image\" />");
            sb.AppendLine($"  <meta property=\"twitter:url\" content=\"{shareUrl}\" />");
            sb.AppendLine($"  <meta property=\"twitter:title\" content=\"Mình đã đạt {score}/{total} điểm!\" />");
            sb.AppendLine($"  <meta property=\"twitter:description\" content=\"Bài trắc nghiệm về {topic}! Bạn có thể làm tốt hơn không. Nhấn để xem chi tiết và thử sức!\" />");
            sb.AppendLine($"  <meta property=\"twitter:image\" content=\"{previewImage}\" />");
            sb.AppendLine($"  <meta http-equiv=\"refresh\" content=\"0; url={redirectUrl}\" />");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("  <p>Đang chuyển hướng đến kết quả...</p>");
            sb.AppendLine($"  <h2>Kết quả: {score}/{total} điểm</h2>");
            sb.AppendLine($"  <h3>Chủ đề: {topic}</h3>");
            sb.AppendLine($"  <script type=\"text/javascript\">window.location.href = '{redirectUrl}';</script>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return Content(sb.ToString(), "text/html; charset=utf-8");
        }

        private static string Render404()
        {
            return "<!DOCTYPE html><html><head><meta charset=\"utf-8\" /><title>404 Not Found</title></head><body><h1>404 - Quiz result not found</h1></body></html>";
        }
    }
}
