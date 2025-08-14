using Microsoft.EntityFrameworkCore;
using QuizGameServer.Domain.Entities;

namespace QuizGameServer.Infrastructure
{
    public class QuizGameDbContext : DbContext
    {
        public QuizGameDbContext(DbContextOptions<QuizGameDbContext> options) : base(options) { }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<SharedQuiz> SharedQuizzes { get; set; }
        public DbSet<QuizContent> QuizContents { get; set; }
        public DbSet<QuizContentQuestion> QuizContentQuestions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Configurations.UserProfileConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
