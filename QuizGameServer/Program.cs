using DotnetGeminiSDK;
using DotnetGeminiSDK.Client;
using DotnetGeminiSDK.Client.Interfaces;
using QuizGameServer.Configurations;
using QuizGameServer.Middlewares;
using QuizGameServer.Services;
using Serilog;
using Serilog.Events;
using System.Net;

namespace QuizGameServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var logger = builder.Logging.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

            var loggerConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext();

            if (builder.Environment.IsProduction())
            {
                var emailOptions = new SerilogEmailOptions();
                builder.Configuration.GetSection("Serilog:WriteTo:2:Args").Bind(emailOptions);

                loggerConfig.WriteTo.Email(
                    from: emailOptions.FromEmail,
                    to: emailOptions.ToEmail,
                    host: emailOptions.MailServer,
                    port: emailOptions.Port,
                    credentials: new NetworkCredential(emailOptions.NetworkCredentials?.UserName, emailOptions.NetworkCredentials?.Password),
                    subject: emailOptions.EmailSubject,
                    restrictedToMinimumLevel: LogEventLevel.Error
                );
            }

            Log.Logger = loggerConfig.CreateLogger();
            Serilog.Debugging.SelfLog.Enable(Console.Error);

            builder.Host.UseSerilog();

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient<GeminiService>();
            builder.Services.AddLogging();
            builder.Services.Configure<GeminiOptions>(builder.Configuration.GetSection("Gemini"));

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

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthorization();
            app.MapControllers();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            try
            {
                Log.Information("Starting up...");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed!");
            }
        }
    }
}
