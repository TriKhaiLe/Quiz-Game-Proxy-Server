using System.ComponentModel.DataAnnotations;

namespace QuizGameServer.Application.Contracts
{
    public class UserProfileRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string AvatarId { get; set; }
    }
}
