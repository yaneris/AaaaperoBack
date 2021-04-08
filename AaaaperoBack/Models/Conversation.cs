using System.ComponentModel.DataAnnotations;

namespace AaaaperoBack.Models
{
    // Conversation model to group messages between an employer and a candidate
    public class Conversation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int EmployerId { get; set; }
        [Required]
        public int CandidateId { get; set; }
    }
}