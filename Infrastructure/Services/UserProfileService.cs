using QuizGameServer.Application.Contracts;
using QuizGameServer.Application.Interfaces;
using QuizGameServer.Domain.Entities;

namespace QuizGameServer.Infrastructure.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly QuizGameDbContext _dbContext;

        public UserProfileService(QuizGameDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserProfile> GetProfileAsync(string userId)
        {
            return await _dbContext.UserProfiles.FindAsync(userId);
        }

        public async Task<UserProfile> UpsertProfileAsync(string userId, UserProfileRequest request)
        {
            var profile = await _dbContext.UserProfiles.FindAsync(userId);
            if (profile == null)
            {
                profile = new UserProfile
                {
                    UserId = userId,
                    Username = request.Username,
                    AvatarId = request.AvatarId
                };
                _dbContext.UserProfiles.Add(profile);
            }
            else
            {
                profile.Username = request.Username;
                profile.AvatarId = request.AvatarId;
                _dbContext.UserProfiles.Update(profile);
            }
            await _dbContext.SaveChangesAsync();
            return profile;
        }
    }
}
