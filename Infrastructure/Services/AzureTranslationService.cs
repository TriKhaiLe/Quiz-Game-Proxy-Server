using Application.Configurations;
using Application.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AzureTranslationService : ITranslationService
    {
        private readonly AzureTranslatorOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;

        public AzureTranslationService(IOptions<AzureTranslatorOptions> options, IHttpClientFactory httpClientFactory)
        {
            _options = options.Value;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> TranslateTextAsync(string text, string targetLanguage, string sourceLanguage = null)
        {
            var result = await TranslateWithDetailsAsync(text, targetLanguage, sourceLanguage);
            return result.TranslatedText;
        }

        public async Task<TranslationResult> TranslateWithDetailsAsync(string text, string targetLanguage, string sourceLanguage = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text to translate cannot be empty", nameof(text));
            }

            if (string.IsNullOrWhiteSpace(targetLanguage))
            {
                throw new ArgumentException("Target language cannot be empty", nameof(targetLanguage));
            }

            var client = _httpClientFactory.CreateClient();

            // Construct the URL
            var route = $"/translate?api-version=3.0&to={targetLanguage}";
            if (!string.IsNullOrWhiteSpace(sourceLanguage))
            {
                route += $"&from={sourceLanguage}";
            }

            var endpoint = _options.Endpoint.TrimEnd('/');
            var requestUri = $"{endpoint}{route}";

            // Prepare the request body
            var requestBody = new object[] { new { Text = text } };
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            // Set required headers
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _options.Key);
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", _options.Region);

            // Send the request
            var response = await client.PostAsync(requestUri, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Translation API request failed with status {response.StatusCode}: {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var translationResponse = JsonConvert.DeserializeObject<List<TranslationApiResponse>>(responseContent);

            if (translationResponse == null || !translationResponse.Any() || !translationResponse[0].Translations.Any())
            {
                throw new InvalidOperationException("No translation results returned from the API");
            }

            var firstResult = translationResponse[0];
            var firstTranslation = firstResult.Translations[0];

            return new TranslationResult
            {
                TranslatedText = firstTranslation.Text,
                DetectedSourceLanguage = firstResult.DetectedLanguage?.Language ?? sourceLanguage ?? "unknown",
                ConfidenceScore = firstResult.DetectedLanguage?.Score ?? 1.0
            };
        }

        // Response model classes for Azure Translator API
        private class TranslationApiResponse
        {
            public DetectedLanguage DetectedLanguage { get; set; }
            public List<Translation> Translations { get; set; } = new List<Translation>();
        }

        private class DetectedLanguage
        {
            public string Language { get; set; } = string.Empty;
            public double Score { get; set; }
        }

        private class Translation
        {
            public string Text { get; set; } = string.Empty;
            public string To { get; set; } = string.Empty;
        }
    }
}
