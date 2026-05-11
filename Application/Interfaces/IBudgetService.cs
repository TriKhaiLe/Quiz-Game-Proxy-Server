using QuizGameServer.Application.Contracts;

namespace QuizGameServer.Application.Interfaces
{
    public interface IBudgetService
    {
        Task<List<BudgetMonthSummaryDto>> GetMonthsAsync(string userId);
        Task<BudgetStateResponse> GetStateAsync(string userId, string month);
        Task<(BudgetStateResponse Response, BudgetStateResponse ServerState)> UpsertStateAsync(string userId, string month, BudgetStateUpdateRequest request);
        Task<BudgetStateResponse> StartNextMonthAsync(string userId, string fromMonth);
        Task<BudgetSnapshotResponse> UpsertSnapshotAsync(string userId, BudgetSnapshotRequest request);
        Task<BudgetSnapshotResponse> GetSnapshotAsync(string userId, string month);
    }
}
