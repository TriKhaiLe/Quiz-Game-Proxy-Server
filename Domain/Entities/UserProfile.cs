using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizGameServer.Domain.Entities
{
    [Table("UserProfiles")]
    public class UserProfile
    {
        [Key]
        public string UserId { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string AvatarId { get; set; }
    }
}
