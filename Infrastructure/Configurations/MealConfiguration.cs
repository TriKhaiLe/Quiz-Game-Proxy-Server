using Dishboard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dishboard.Infrastructure.Configurations
{
//     public class Meal
// {
//     public int Id { get; set; }

//     public string Name { get; set; } = string.Empty;

//     public int Calories { get; set; }

//     public decimal Price { get; set; }

//     public bool IsFavorite { get; set; }
// }

    public class MealConfiguration : IEntityTypeConfiguration<Meal>
    {
        public void Configure(EntityTypeBuilder<Meal> builder)
        {
            builder.Property(m => m.Name).IsRequired().HasMaxLength(100);
        }
    }
}
