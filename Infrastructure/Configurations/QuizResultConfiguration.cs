using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizGameServer.Domain.Entities;

namespace QuizGameServer.Infrastructure.Configurations
{
    public class QuizResultConfiguration : IEntityTypeConfiguration<QuizResult>
    {
        public void Configure(EntityTypeBuilder<QuizResult> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Topic).IsRequired();
            builder.Property(x => x.Difficulty).IsRequired();
            builder.Property(x => x.UserAnswers).IsRequired();
            builder.HasOne(x => x.QuizContent)
                   .WithMany()
                   .HasForeignKey(x => x.QuizContentId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
