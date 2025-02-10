
using QuizGameServer.Configurations;
using QuizGameServer.Services;

namespace QuizGameServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient<GoogleGenerativeAIService>();
            builder.Services.AddLogging();

            var apiKey = builder.Configuration["Gemini:ApiKey"] ?? Environment.GetEnvironmentVariable("GOOGLE_GEMINI_API_KEY");
            builder.Services.Configure<GoogleGenerativeAIOptions>(options => options.ApiKey = apiKey);


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
