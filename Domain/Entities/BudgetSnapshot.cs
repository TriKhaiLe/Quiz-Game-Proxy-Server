using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizGameServer.Domain.Entities
{
    [Table("BudgetSnapshots")]
    public class BudgetSnapshot
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Month { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public string SnapshotJson { get; set; }
    }
}
