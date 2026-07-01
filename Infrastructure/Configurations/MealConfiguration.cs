using Dishboard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dishboard.Infrastructure.Configurations
{
    public class MealConfiguration : IEntityTypeConfiguration<Meal>
    {
        public void Configure(EntityTypeBuilder<Meal> builder)
        {
            builder.Property(m => m.Name).IsRequired().HasMaxLength(100);
        }
    }

    public class MealCategoryConfiguration : IEntityTypeConfiguration<MealCategory>
    {
        public void Configure(EntityTypeBuilder<MealCategory> builder)
        {
            builder.Property(mc => mc.Name).IsRequired().HasMaxLength(100);
        }
    }
}
