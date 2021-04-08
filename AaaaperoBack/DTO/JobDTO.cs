using System.ComponentModel.DataAnnotations;

namespace AaaaperoBack.DTO
{
    public class JobDTO
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

    }
}