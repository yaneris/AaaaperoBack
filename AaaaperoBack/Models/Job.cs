using System.ComponentModel.DataAnnotations;

namespace AaaaperoBack.Models
{
    // Jobs shown in the employer page. Can be advertised if employer pays premium fee
    public class Job
    {
        [Key]
        public int Id { get; set; }
        public int EmployerId { get; set; }
        public int CandidateId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Remuneration { get; set; }
        
    }
}