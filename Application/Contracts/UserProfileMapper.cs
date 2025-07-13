using AutoMapper;
using QuizGameServer.Domain.Entities;

namespace QuizGameServer.Application.Contracts
{
    public class UserProfileMappingProfile : Profile
    {
        public UserProfileMappingProfile()
        {
            CreateMap<UserProfile, UserProfileDto>();
            CreateMap<UserProfileDto, UserProfile>();
        }
    }
}
