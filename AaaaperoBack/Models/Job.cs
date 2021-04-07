using System.ComponentModel.DataAnnotations;

namespace AaaaperoBack.Models
{
    public class Job
    {
        [Key]
        public int Id { get; set; }
        public int EmployerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        
        public bool PremiumAdvertisement { get; set; }
    }
}