namespace Dishboard.Domain.Entities;

public class Meal
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Calories { get; set; }

    public decimal Price { get; set; }

    public bool IsFavorite { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int MealCategoryId { get; set; } = 1;

    public MealCategory? MealCategory { get; set; }
}
