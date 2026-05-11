using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using QuizGameServer.Application.Contracts;
using QuizGameServer.Application.Interfaces;
using QuizGameServer.Domain.Entities;

namespace QuizGameServer.Infrastructure.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly QuizGameDbContext _dbContext;
        private readonly JsonSerializerOptions _jsonOptions;

        public BudgetService(QuizGameDbContext dbContext)
        {
            _dbContext = dbContext;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<BudgetMonthSummaryDto>> GetMonthsAsync(string userId)
        {
            var months = await _dbContext.BudgetMonthStates
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Month)
                .Select(x => new BudgetMonthSummaryDto
                {
                    Month = x.Month,
                    UpdatedAt = x.UpdatedAt,
                    Label = BuildMonthLabel(x.Month)
                })
                .ToListAsync();

            return months;
        }

        public async Task<BudgetStateResponse> GetStateAsync(string userId, string month)
        {
            var state = await _dbContext.BudgetMonthStates
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Month == month);

            if (state == null)
            {
                return null;
            }

            return new BudgetStateResponse
            {
                Month = state.Month,
                Version = state.Version,
                UpdatedAt = state.UpdatedAt,
                State = DeserializeState(state.StateJson)
            };
        }

        public async Task<(BudgetStateResponse Response, BudgetStateResponse ServerState)> UpsertStateAsync(string userId, string month, BudgetStateUpdateRequest request)
        {
            var existing = await _dbContext.BudgetMonthStates
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Month == month);

            if (existing != null && request.Version != existing.Version)
            {
                return (null, new BudgetStateResponse
                {
                    Month = existing.Month,
                    Version = existing.Version,
                    UpdatedAt = existing.UpdatedAt,
                    State = DeserializeState(existing.StateJson)
                });
            }

            var now = DateTime.UtcNow;

            if (existing == null)
            {
                existing = new BudgetMonthState
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Month = month,
                    Version = Math.Max(1, request.Version + 1),
                    UpdatedAt = now,
                    StateJson = SerializeState(request.State)
                };
                _dbContext.BudgetMonthStates.Add(existing);
            }
            else
            {
                existing.Version += 1;
                existing.UpdatedAt = now;
                existing.StateJson = SerializeState(request.State);
                _dbContext.BudgetMonthStates.Update(existing);
            }

            await _dbContext.SaveChangesAsync();

            return (new BudgetStateResponse
            {
                Month = existing.Month,
                Version = existing.Version,
                UpdatedAt = existing.UpdatedAt,
                State = DeserializeState(existing.StateJson)
            }, null);
        }

        public async Task<BudgetStateResponse> StartNextMonthAsync(string userId, string fromMonth)
        {
            var current = await _dbContext.BudgetMonthStates
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Month == fromMonth);

            if (current == null)
            {
                return null;
            }

            if (!TryParseMonth(fromMonth, out var currentMonthDate))
            {
                return null;
            }

            var nextMonthDate = currentMonthDate.AddMonths(1);
            var nextMonth = nextMonthDate.ToString("yyyy-MM", CultureInfo.InvariantCulture);

            var existingNextMonth = await _dbContext.BudgetMonthStates
                .AnyAsync(x => x.UserId == userId && x.Month == nextMonth);

            if (existingNextMonth)
            {
                return null;
            }

            var currentState = DeserializeState(current.StateJson) ?? new BudgetStateDto();

            var newState = new BudgetStateDto
            {
                MoneySources = currentState.MoneySources
                    .Select(source => new BudgetMoneySourceDto
                    {
                        Id = source.Id,
                        Name = source.Name,
                        Budget = source.Balance,
                        Spent = 0,
                        Balance = source.Balance
                    })
                    .ToList(),
                Templates = new List<BudgetTemplateDto>(),
                History = new List<BudgetHistoryEntryDto>
                {
                    new BudgetHistoryEntryDto
                    {
                        Id = Guid.NewGuid().ToString(),
                        Description = $"Started new month ({nextMonthDate.ToString("MMMM yyyy", CultureInfo.InvariantCulture)})",
                        Timestamp = nextMonthDate
                    }
                },
                BudgetLog = new List<BudgetLogEntryDto>
                {
                    new BudgetLogEntryDto
                    {
                        Id = Guid.NewGuid().ToString(),
                        Description = "Last month balance",
                        Changes = currentState.MoneySources.ToDictionary(source => source.Id, source => source.Balance),
                        IsInitial = true,
                        CreatedAt = nextMonthDate
                    }
                },
                BudgetLogSnapshot = null,
                BudgetLogBalanceLocks = new Dictionary<string, bool>(),
                CurrentMonth = nextMonthDate,
                MonthDescription = string.Empty
            };

            var nextStateEntity = new BudgetMonthState
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Month = nextMonth,
                Version = 1,
                UpdatedAt = DateTime.UtcNow,
                StateJson = SerializeState(newState)
            };

            _dbContext.BudgetMonthStates.Add(nextStateEntity);
            await _dbContext.SaveChangesAsync();

            return new BudgetStateResponse
            {
                Month = nextStateEntity.Month,
                Version = nextStateEntity.Version,
                UpdatedAt = nextStateEntity.UpdatedAt,
                State = newState
            };
        }

        public async Task<BudgetSnapshotResponse> UpsertSnapshotAsync(string userId, BudgetSnapshotRequest request)
        {
            var existing = await _dbContext.BudgetSnapshots
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Month == request.Month);

            var createdAt = request.Snapshot?.CreatedAt ?? DateTime.UtcNow;
            var snapshotJson = JsonSerializer.Serialize(request.Snapshot, _jsonOptions);

            if (existing == null)
            {
                existing = new BudgetSnapshot
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Month = request.Month,
                    CreatedAt = createdAt,
                    SnapshotJson = snapshotJson
                };
                _dbContext.BudgetSnapshots.Add(existing);
            }
            else
            {
                existing.CreatedAt = createdAt;
                existing.SnapshotJson = snapshotJson;
                _dbContext.BudgetSnapshots.Update(existing);
            }

            await _dbContext.SaveChangesAsync();

            return new BudgetSnapshotResponse
            {
                Month = request.Month,
                Snapshot = DeserializeSnapshot(existing.SnapshotJson)
            };
        }

        public async Task<BudgetSnapshotResponse> GetSnapshotAsync(string userId, string month)
        {
            var snapshot = await _dbContext.BudgetSnapshots
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Month == month);

            if (snapshot == null)
            {
                return null;
            }

            return new BudgetSnapshotResponse
            {
                Month = snapshot.Month,
                Snapshot = DeserializeSnapshot(snapshot.SnapshotJson)
            };
        }

        private string BuildMonthLabel(string month)
        {
            return TryParseMonth(month, out var date)
                ? date.ToString("MMMM yyyy", CultureInfo.InvariantCulture)
                : month;
        }

        private bool TryParseMonth(string month, out DateTime date)
        {
            return DateTime.TryParseExact(
                month,
                "yyyy-MM",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out date);
        }

        private string SerializeState(BudgetStateDto state)
        {
            return JsonSerializer.Serialize(state, _jsonOptions);
        }

        private BudgetStateDto DeserializeState(string json)
        {
            return JsonSerializer.Deserialize<BudgetStateDto>(json, _jsonOptions);
        }

        private BudgetLogSnapshotDto DeserializeSnapshot(string json)
        {
            return JsonSerializer.Deserialize<BudgetLogSnapshotDto>(json, _jsonOptions);
        }
    }
}
