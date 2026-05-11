using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizGameServer.Domain.Entities;

namespace QuizGameServer.Infrastructure.Configurations
{
    public class BudgetSnapshotConfiguration : IEntityTypeConfiguration<BudgetSnapshot>
    {
        public void Configure(EntityTypeBuilder<BudgetSnapshot> builder)
        {
            builder.ToTable("BudgetSnapshots");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Month).IsRequired().HasMaxLength(7);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.SnapshotJson).IsRequired().HasColumnType("jsonb");
            builder.HasIndex(x => new { x.UserId, x.Month }).IsUnique();
        }
    }
}
