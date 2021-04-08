using System.ComponentModel.DataAnnotations;

namespace AaaaperoBack.Models
{
    public enum OfferStatus
    {
        Pending,
        Accepted,
        Rejected,
    }
    
    public class Offer
    {
        [Key]
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public int EmployerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Remuneration { get; set; }
        public OfferStatus Status { get; set; }

        public Offer()
        {
            Status = OfferStatus.Pending;
        }
    }
}