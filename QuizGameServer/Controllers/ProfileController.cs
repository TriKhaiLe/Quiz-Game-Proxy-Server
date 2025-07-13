using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QuizGameServer.Application.Contracts;
using System.Security.Claims;
using QuizGameServer.Application.Interfaces;

namespace QuizGameServer.Controllers
{
    [ApiController]
    [Route("api/profile")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IUserProfileService _profileService;
        private readonly IMapper _mapper;

        public ProfileController(IUserProfileService profileService, IMapper mapper)
        {
            _profileService = profileService;
            _mapper = mapper;
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
            var dto = _mapper.Map<QuizGameServer.Application.Contracts.UserProfileDto>(profile);
            return Ok(dto);
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
            var dto = _mapper.Map<QuizGameServer.Application.Contracts.UserProfileDto>(profile);
            return Ok(dto);
        }
    }
}
