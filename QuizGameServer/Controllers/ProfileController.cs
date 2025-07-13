using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QuizGameServer.Models;
using QuizGameServer.Services;
using System.Linq;
using System.Security.Claims;

namespace QuizGameServer.Controllers
{
    [ApiController]
    [Route("api/profile")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly UserProfileService _profileService;

        public ProfileController(UserProfileService profileService)
        {
            _profileService = profileService;
        }

        // Helper to extract user id from JWT claims
        private string GetUserId()
        {
            // Use sub claim as user id (standard in JWT)
            return User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;
        }


        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var profile = await _profileService.GetProfileAsync(userId);
            if (profile == null)
            {
                return NotFound(new { error = "Profile not found for this user." });
            }
            return Ok(profile);
        }

        [HttpPut]
        public async Task<IActionResult> UpsertProfile([FromBody] UserProfileRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    errors = ModelState.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray())
                });
            }
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var profile = await _profileService.UpsertProfileAsync(userId, request);
            return Ok(profile);
        }
    }
}
