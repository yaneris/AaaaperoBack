using System.ComponentModel.DataAnnotations;

namespace AaaaperoBack.Models
{
    public class Offer
    {
        [Key]
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public int EmployerId { get; set; }
        public int JobId { get; set; }
    }
}