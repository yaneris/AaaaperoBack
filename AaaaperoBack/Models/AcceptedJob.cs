using System.ComponentModel.DataAnnotations;

namespace AaaaperoBack.Models
{
    public class AcceptedJob
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
        
        [Key]
        public int Id { get; set; }
        public AcceptedJobStatus Status { get; set; }
        public Rating CandidateRating { get; set; }
        public Rating EmployerRating { get; set; }
        public int Remuneration { get; set; }
        public int EmployerId { get; set; }
        public int CandidateId { get; set; }

        public AcceptedJob()
        {
            Status = AcceptedJobStatus.Progress;
        }
    }
}