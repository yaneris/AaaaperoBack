using System.ComponentModel.DataAnnotations;

namespace AaaaperoBack.Models
{
    public class Conversation
    {
        [Key]
        public int Id { get; set; }
        public int EmployerId { get; set; }
        public int CandidateId { get; set; }
    }
}