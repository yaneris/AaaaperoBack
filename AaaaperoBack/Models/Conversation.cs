using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AaaaperoBack.Models
{
    // Conversation model to group messages between an employer and a candidate
    public class Conversation
    {
        [Key]
        public int Id { get; set; }
        public int EmployerId { get; set; }
        public int CandidateId { get; set; }
        public List<Message> Messages { get; set; }
    }
}