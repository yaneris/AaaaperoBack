using System.ComponentModel.DataAnnotations;

namespace AaaaperoBack.Models
{
    public enum AcceptedJobStatus
    {
        Progress,
        Done,
        Paid,
    }

    public enum Rating
    {
        One=1,
        Two=2,
        Three=3,
        Four=4,
        Five=5,
    }
    
    // AcceptedJob is a Job accepted by both the employer and the candidate and WIP
    // Rating allowed after AcceptedJob Status is "done" or "paid"
    public class AcceptedJob
    {
        
        [Key]
        public int Id { get; set; }
        public AcceptedJobStatus Status { get; set; }
        public Rating CandidateRating { get; set; }
        public Rating EmployerRating { get; set; }
        [Required]
        public int Remuneration { get; set; }
        [Required]
        public int EmployerId { get; set; }
        [Required]
        public int CandidateId { get; set; }

        public AcceptedJob()
        {
            Status = AcceptedJobStatus.Progress;
        }
    }
}