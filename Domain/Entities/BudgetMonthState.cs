using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizGameServer.Domain.Entities
{
    [Table("BudgetMonthStates")]
    public class BudgetMonthState
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Month { get; set; }
        [Required]
        public int Version { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
        [Required]
        public string StateJson { get; set; }
    }
}
