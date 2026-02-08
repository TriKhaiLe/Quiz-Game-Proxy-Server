using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace QuizGameServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranslationController : ControllerBase
    {
        private readonly ITranslationService _translationService;

        public TranslationController(ITranslationService translationService)
        {
            _translationService = translationService;
        }

        /// <summary>
        /// Translate text to a target language
        /// </summary>
        /// <param name="request">Translation request containing text and language parameters</param>
        /// <returns>Translated text</returns>
        [HttpPost("translate")]
        public async Task<IActionResult> Translate([FromBody] TranslationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var translatedText = await _translationService.TranslateTextAsync(
                    request.Text,
                    request.TargetLanguage,
                    request.SourceLanguage
                );

                return Ok(new { translatedText });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Translation failed", details = ex.Message });
            }
        }

        /// <summary>
        /// Translate text with detailed information including detected language and confidence score
        /// </summary>
        /// <param name="request">Translation request containing text and language parameters</param>
        /// <returns>Translation result with details</returns>
        [HttpPost("translate-detailed")]
        public async Task<IActionResult> TranslateDetailed([FromBody] TranslationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _translationService.TranslateWithDetailsAsync(
                    request.Text,
                    request.TargetLanguage,
                    request.SourceLanguage
                );

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Translation failed", details = ex.Message });
            }
        }
    }

    public class TranslationRequest
    {
        [Required(ErrorMessage = "Text is required")]
        public string Text { get; set; } = string.Empty;

        [Required(ErrorMessage = "Target language is required")]
        [RegularExpression(@"^[a-z]{2}(-[A-Z]{2})?$", ErrorMessage = "Invalid language code format. Examples: 'en', 'vi', 'en-US'")]
        public string TargetLanguage { get; set; } = string.Empty;

        [RegularExpression(@"^[a-z]{2}(-[A-Z]{2})?$", ErrorMessage = "Invalid language code format. Examples: 'en', 'vi', 'en-US'")]
        public string? SourceLanguage { get; set; }
    }
}
