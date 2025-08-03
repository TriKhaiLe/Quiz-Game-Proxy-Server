using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using QuizGameServer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class QuizGameDbContextFactory : IDesignTimeDbContextFactory<QuizGameDbContext>
    {
        public QuizGameDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "QuizGameServer");

            Console.WriteLine($"[DbContextFactory] Base path resolved to: {basePath}");
            if (!Directory.Exists(basePath))
                Console.WriteLine($"[DbContextFactory] Warning: basePath directory does not exist!");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("[DbContextFactory] Connection string is missing. Check appsettings or environment variables.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<QuizGameDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new QuizGameDbContext(optionsBuilder.Options);
        }
    }
}
