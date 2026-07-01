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
        public DbSet<MealCategory> MealCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                    typeof(QuizGameDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
