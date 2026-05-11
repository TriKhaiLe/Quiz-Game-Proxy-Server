using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITranslationService
    {
        Task<string> TranslateTextAsync(string text, string targetLanguage, string sourceLanguage = null);
        Task<TranslationResult> TranslateWithDetailsAsync(string text, string targetLanguage, string sourceLanguage = null);
    }

    public class TranslationResult
    {
        public string TranslatedText { get; set; } = string.Empty;
        public string DetectedSourceLanguage { get; set; } = string.Empty;
        public double ConfidenceScore { get; set; }
    }
}
