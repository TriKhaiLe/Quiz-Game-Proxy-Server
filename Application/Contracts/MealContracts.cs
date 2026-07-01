namespace Dishboard.Application.Contracts;

public class MealInput
{
    public string Name { get; set; } = string.Empty;
    public int Calories { get; set; }
    public decimal Price { get; set; }
    public bool IsFavorite { get; set; }

}

public class MealResponse
{
    public string Name { get; set; } = string.Empty;
    public int Calories { get; set; }
    public decimal Price { get; set; }
    public bool IsFavorite { get; set; }

}

public class MealCategoryInput
{
    public string Name { get; set; } = string.Empty;
}