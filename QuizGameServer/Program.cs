using DotnetGeminiSDK;
using DotnetGeminiSDK.Client;
using DotnetGeminiSDK.Client.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuizGameServer.Application.Configurations;
using QuizGameServer.Application.Contracts;
using QuizGameServer.Application.Interfaces;
using QuizGameServer.Infrastructure;
using QuizGameServer.Infrastructure.Services;
using QuizGameServer.Middlewares;
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
            builder.Services.AddAutoMapper(typeof(UserProfileMappingProfile));


            // Add EF Core PostgreSQL
            builder.Services.AddDbContext<QuizGameDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add authentication for Supabase JWT
            var supabaseJwtSecret = builder.Configuration["Supabase:JwtSecret"];
            var supabaseTokenExpiry = builder.Configuration.GetValue<int>("Supabase:AccessTokenExpiry", 3600);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false, // Supabase JWTs are self-contained
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromSeconds(supabaseTokenExpiry),
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(supabaseJwtSecret ?? ""))
                    };
                });

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            builder.Services.AddHttpClient<GeminiService>();
            builder.Services.AddLogging();
            builder.Services.Configure<GeminiOptions>(builder.Configuration.GetSection("Gemini"));

            var apiKey = builder.Configuration["Gemini:ApiKey"] ?? Environment.GetEnvironmentVariable("GOOGLE_GEMINI_API_KEY");

            if (string.IsNullOrEmpty(apiKey))
            {
                logger.LogWarning("API Key for Gemini is missing! The application may not function properly.");
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
            builder.Services.AddScoped<IUserProfileService, UserProfileService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });

                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(
                        "http://localhost:5173", // Cho local dev
                        "https://quiz-game-trivia-master.vercel.app" // Cho bản đã deploy
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                c.InjectJavascript("/swagger-ui/custom.js");
            });

            app.UseHttpsRedirection();
            app.UseCors(builder.Environment.IsDevelopment() ? "AllowAll" : "AllowFrontend");
            app.UseAuthentication();
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
