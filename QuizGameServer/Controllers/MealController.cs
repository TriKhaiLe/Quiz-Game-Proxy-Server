using Dishboard.Application.Contracts;
using Dishboard.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizGameServer.Infrastructure;

namespace Dishboard.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MealController : ControllerBase
{
    private readonly QuizGameDbContext _dbContext;

    public MealController(QuizGameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region CRUD Operations
    [HttpGet]
    public async Task<IActionResult> GetMeals(string sort = "price", string order = "asc")
    {
        IQueryable<Meal> query = _dbContext.Meals;
        query = sort.ToLower() switch
        {
            "price" => order.ToLower() == "asc" ? query.OrderBy(m => m.Price) : query.OrderByDescending(m => m.Price),
            "calories" => order.ToLower() == "asc" ? query.OrderBy(m => m.Calories) : query.OrderByDescending(m => m.Calories),
            _ => order.ToLower() == "asc" ? query.OrderBy(m => m.Id) : query.OrderByDescending(m => m.Id),
        };

        var meals = await query.ToListAsync();
        return Ok(meals);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetMeal(int id)
    {
        var meal = await _dbContext.Meals.FirstOrDefaultAsync(m => m.Id == id);
        if (meal == null)
        {
            return NotFound();
        }
        return Ok(meal);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMeal([FromBody] MealInput meal)
    {
        var newMeal = new Meal
        {
            Name = meal.Name,
            Calories = meal.Calories,
            Price = meal.Price,
            IsFavorite = meal.IsFavorite
        };
        _dbContext.Meals.Add(newMeal);
        await _dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMeal), new { id = newMeal.Id }, newMeal);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMeal(int id, [FromBody] MealInput meal)
    {
        if (id < 0)
        {
            return BadRequest();
        }

        var existingMeal = await _dbContext.Meals.FindAsync(id);
        if (existingMeal == null)
        {
            return NotFound();
        }

        existingMeal.Name = meal.Name;
        existingMeal.Calories = meal.Calories;
        existingMeal.Price = meal.Price;
        existingMeal.IsFavorite = meal.IsFavorite;
        await _dbContext.SaveChangesAsync();
        return Ok(existingMeal);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMeal(int id)
    {
        var meal = await _dbContext.Meals.FindAsync(id);
        if (meal == null)
        {
            return NotFound();
        }

        _dbContext.Meals.Remove(meal);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

    #endregion

    #region LINQ Queries

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Meal>>> SearchMealsByName(string keyword)
    {
        var meals = await _dbContext.Meals.Where(m => m.Name.Contains(keyword))
            .ToListAsync();

        return Ok(meals);
    }

    [HttpGet("favorites")]

    public async Task<ActionResult<IEnumerable<Meal>>> GetFavoriteMeals()
    {
        var meals = await _dbContext.Meals.Where(m => m.IsFavorite).ToListAsync();
        return Ok(meals);
    }

    [HttpGet("calories")]
    public async Task<ActionResult<IEnumerable<Meal>>> GetMealsUnderCalories(int max)
    {
        var meals = await _dbContext.Meals.Where(m => m.Calories < max).ToListAsync();
        return Ok(meals);
    }

    #endregion

}