using Dishboard.Domain.Entities;
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
        public DbSet<QuizResult> QuizResults { get; set; }
        public DbSet<BudgetMonthState> BudgetMonthStates { get; set; }
        public DbSet<BudgetSnapshot> BudgetSnapshots { get; set; }

        public DbSet<Meal> Meals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Configurations.UserProfileConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.QuizResultConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.BudgetMonthStateConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.BudgetSnapshotConfiguration());
            modelBuilder.ApplyConfiguration(new Dishboard.Infrastructure.Configurations.MealConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
