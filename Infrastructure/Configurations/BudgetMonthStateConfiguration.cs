using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizGameServer.Domain.Entities;

namespace QuizGameServer.Infrastructure.Configurations
{
    public class BudgetMonthStateConfiguration : IEntityTypeConfiguration<BudgetMonthState>
    {
        public void Configure(EntityTypeBuilder<BudgetMonthState> builder)
        {
            builder.ToTable("BudgetMonthStates");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Month).IsRequired().HasMaxLength(7);
            builder.Property(x => x.Version).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Property(x => x.StateJson).IsRequired().HasColumnType("jsonb");
            builder.HasIndex(x => new { x.UserId, x.Month }).IsUnique();
        }
    }
}
