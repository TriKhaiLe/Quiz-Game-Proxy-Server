using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizGameServer.Application.Contracts;
using QuizGameServer.Application.Interfaces;
using System.Security.Claims;

namespace QuizGameServer.Controllers
{
    [ApiController]
    [Route("api/budget")]
    [Authorize]
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetService _budgetService;

        public BudgetController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        private string GetUserId()
        {
            return User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;
        }

        [HttpGet("months")]
        public async Task<IActionResult> GetMonths()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    error = new
                    {
                        code = "UNAUTHORIZED",
                        message = "Missing or invalid credentials"
                    }
                });
            }

            var months = await _budgetService.GetMonthsAsync(userId);
            return Ok(new BudgetMonthsResponse { Months = months });
        }

        [HttpGet("state")]
        public async Task<IActionResult> GetState([FromQuery] string month)
        {
            if (string.IsNullOrWhiteSpace(month))
            {
                return BadRequest(new
                {
                    error = new
                    {
                        code = "VALIDATION_FAILED",
                        message = "Month is required"
                    }
                });
            }

            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    error = new
                    {
                        code = "UNAUTHORIZED",
                        message = "Missing or invalid credentials"
                    }
                });
            }

            var state = await _budgetService.GetStateAsync(userId, month);
            if (state == null)
            {
                return NotFound(new
                {
                    error = new
                    {
                        code = "NOT_FOUND",
                        message = $"No budget state found for month {month}"
                    }
                });
            }

            return Ok(state);
        }

        [HttpPut("state")]
        public async Task<IActionResult> UpsertState([FromQuery] string month, [FromBody] BudgetStateUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(month) || request == null)
            {
                return BadRequest(new
                {
                    error = new
                    {
                        code = "VALIDATION_FAILED",
                        message = "Month and state payload are required"
                    }
                });
            }

            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    error = new
                    {
                        code = "UNAUTHORIZED",
                        message = "Missing or invalid credentials"
                    }
                });
            }

            var (response, serverState) = await _budgetService.UpsertStateAsync(userId, month, request);
            if (serverState != null)
            {
                return Conflict(new
                {
                    error = new
                    {
                        code = "VERSION_CONFLICT",
                        message = "State has been updated by another device",
                        details = new
                        {
                            serverVersion = serverState.Version,
                            serverUpdatedAt = serverState.UpdatedAt,
                            serverState = serverState.State
                        }
                    }
                });
            }

            return Ok(response);
        }

        [HttpPost("start-next-month")]
        public async Task<IActionResult> StartNextMonth([FromBody] BudgetStartNextMonthRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.FromMonth))
            {
                return BadRequest(new
                {
                    error = new
                    {
                        code = "VALIDATION_FAILED",
                        message = "fromMonth is required"
                    }
                });
            }

            if (!DateTime.TryParseExact(request.FromMonth, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out _))
            {
                return BadRequest(new
                {
                    error = new
                    {
                        code = "VALIDATION_FAILED",
                        message = "fromMonth must be in YYYY-MM format"
                    }
                });
            }

            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    error = new
                    {
                        code = "UNAUTHORIZED",
                        message = "Missing or invalid credentials"
                    }
                });
            }

            var nextMonth = await _budgetService.StartNextMonthAsync(userId, request.FromMonth);
            if (nextMonth == null)
            {
                return NotFound(new
                {
                    error = new
                    {
                        code = "NOT_FOUND",
                        message = $"No budget state found for month {request.FromMonth}"
                    }
                });
            }

            return StatusCode(StatusCodes.Status201Created, new
            {
                createdMonth = nextMonth.Month,
                version = nextMonth.Version,
                state = nextMonth.State
            });
        }

        [HttpPost("snapshot")]
        public async Task<IActionResult> UpsertSnapshot([FromBody] BudgetSnapshotRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Month) || request.Snapshot == null)
            {
                return BadRequest(new
                {
                    error = new
                    {
                        code = "VALIDATION_FAILED",
                        message = "month and snapshot are required"
                    }
                });
            }

            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    error = new
                    {
                        code = "UNAUTHORIZED",
                        message = "Missing or invalid credentials"
                    }
                });
            }

            var response = await _budgetService.UpsertSnapshotAsync(userId, request);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpGet("snapshot")]
        public async Task<IActionResult> GetSnapshot([FromQuery] string month)
        {
            if (string.IsNullOrWhiteSpace(month))
            {
                return BadRequest(new
                {
                    error = new
                    {
                        code = "VALIDATION_FAILED",
                        message = "month is required"
                    }
                });
            }

            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    error = new
                    {
                        code = "UNAUTHORIZED",
                        message = "Missing or invalid credentials"
                    }
                });
            }

            var response = await _budgetService.GetSnapshotAsync(userId, month);
            if (response == null)
            {
                return NotFound(new
                {
                    error = new
                    {
                        code = "NOT_FOUND",
                        message = $"No snapshot found for month {month}"
                    }
                });
            }

            return Ok(response);
        }
    }
}
