using System.ComponentModel.DataAnnotations;

namespace AaaaperoBack.Models
{
    public enum OfferStatus
    {
        Pending,
        Accepted,
        Rejected,
    }
    
    // Offer model: corresponds to a private offer sent to a specific candidate
    public class Offer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CandidateId { get; set; }
        [Required]
        public int EmployerId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Remuneration { get; set; }
        public OfferStatus Status { get; set; }

        public Offer()
        {
            Status = OfferStatus.Pending;
        }
    }
}