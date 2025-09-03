using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace QuizGameServer.Domain.Entities
{
    public class QuizResult
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Topic { get; set; } = string.Empty;
        [Required]
        public int Difficulty { get; set; }
        [Required]
        public List<string> UserAnswers { get; set; } = [];
        [Required]
        public Guid QuizContentId { get; set; }
        public QuizContent QuizContent { get; set; }
    }
}
