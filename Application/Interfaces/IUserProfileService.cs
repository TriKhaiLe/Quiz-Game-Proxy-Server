
using QuizGameServer.Application.Contracts;
using QuizGameServer.Domain.Entities;

namespace QuizGameServer.Application.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfile> GetProfileAsync(string userId);
        Task<UserProfile> UpsertProfileAsync(string userId, UserProfileRequest request);
    }
}
