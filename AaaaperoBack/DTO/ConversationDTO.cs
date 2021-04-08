using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AaaaperoBack.Models;

namespace AaaaperoBack.DTO
{
    public class ConversationDTO
    {
        public int Id { get; set; }
        public int EmployerId { get; set; }
        public string EmployerName { get; set; }
        public int CandidateId { get; set; }
        public string CandidateName { get; set; }
        public List<Message> Messages { get; set; }
    }
}