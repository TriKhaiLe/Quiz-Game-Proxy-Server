using System.ComponentModel.DataAnnotations;

namespace QuizGameServer.Models
{
    public class UserProfileRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string AvatarId { get; set; }
    }
}
