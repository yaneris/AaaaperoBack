using System.ComponentModel.DataAnnotations;

namespace AaaaperoBack.Models
{
    public class Application
    {
        [Key]
        public int Id { get; set;}
        public int JobId { get; set; }
        public int CandidateId { get; set; }
        public string Message { get; set; }
    }
}