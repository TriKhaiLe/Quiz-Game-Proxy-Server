namespace Dishboard.Domain.Entities;

public class MealCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Meal> Meals { get; set; } = [];
}