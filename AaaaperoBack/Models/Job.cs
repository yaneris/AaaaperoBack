using System.ComponentModel.DataAnnotations;

namespace AaaaperoBack.Models
{
    // Jobs shown in the employer page. Can be advertised if employer pays premium fee
    public class Job
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int EmployerId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Remuneration { get; set; }
        
        [Required]
        public bool PremiumAdvertisement { get; set; }

        public Job()
        {
            PremiumAdvertisement = false;
        }
    }
}