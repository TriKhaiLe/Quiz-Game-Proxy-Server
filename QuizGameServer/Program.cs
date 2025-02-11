
using QuizGameServer.Services;
using DotnetGeminiSDK;
using DotnetGeminiSDK.Client.Interfaces;
using DotnetGeminiSDK.Client;

namespace QuizGameServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var logger = builder.Logging.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient<GeminiService>();
            builder.Services.AddLogging();

            var apiKey = builder.Configuration["Gemini:ApiKey"] ?? Environment.GetEnvironmentVariable("GOOGLE_GEMINI_API_KEY");

            if (string.IsNullOrEmpty(apiKey))
            {
                logger.LogWarning("⚠️ API Key for Gemini is missing! The application may not function properly.");
            }
            else
            {
                builder.Services.AddGeminiClient(configure =>
                {
                    configure.ApiKey = apiKey;
                });
            }

            builder.Services.AddTransient<GeminiService>();
            builder.Services.AddTransient<IGeminiClient, GeminiClient>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
