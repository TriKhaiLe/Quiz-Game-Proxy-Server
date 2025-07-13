
using QuizGameServer.Domain.Entities;
using QuizGameServer.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace QuizGameServer.Services
{
    public class UserProfileService
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

        public async Task<UserProfile> UpsertProfileAsync(string userId, Models.UserProfileRequest request)
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
