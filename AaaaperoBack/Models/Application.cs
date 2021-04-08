using System.ComponentModel.DataAnnotations;

namespace AaaaperoBack.Models
{
    // Application model: corresponds to a candidate applying to a Job or an Offer 
    public class Application
    {
        [Key]
        public int Id { get; set;}
        [Required]
        public int JobId { get; set; }
        [Required]
        public int CandidateId { get; set; }
        [Required]
        public string Message { get; set; }
    }
}