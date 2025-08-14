using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace QuizGameServer.Domain.Entities
{
    public class SharedQuiz
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public int CurrentQuestionIndex { get; set; }
        [Required]
        public Guid QuizContentId { get; set; }
        public QuizContent QuizContent { get; set; }
    }

    public class QuizContent
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Topic { get; set; }
        [Required]
        public int Difficulty { get; set; }
        [Required]
        public string ContentHash { get; set; }
        public List<QuizContentQuestion> Questions { get; set; } = new();
    }

    public class QuizContentQuestion
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Question { get; set; }
        [Required]
        public List<string> Options { get; set; } = new();
        [Required]
        public string CorrectAnswer { get; set; }
        [ForeignKey("QuizContent")]
        public Guid QuizContentId { get; set; }
        public QuizContent QuizContent { get; set; }
    }
}
