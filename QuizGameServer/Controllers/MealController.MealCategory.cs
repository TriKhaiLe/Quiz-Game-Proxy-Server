using Dishboard.Application.Contracts;
using Dishboard.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dishboard.Controllers;

public partial class MealController
{
    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<MealCategory>>> GetMealCategories()
    {
        var categories = await _dbContext.MealCategories.AsNoTracking().ToListAsync();
        return Ok(categories);
    }

    [HttpPost("categories")]
    public async Task<IActionResult> CreateMealCategory([FromBody] MealCategoryInput categoryInput)
    {
        var category = new MealCategory
        {
            Name = categoryInput.Name
        };
        {
            _dbContext.MealCategories.Add(category);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMealCategories), new { id = category.Id }, category);
        }
    }
}