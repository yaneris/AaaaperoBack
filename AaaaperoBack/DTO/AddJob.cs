using System.ComponentModel.DataAnnotations;

namespace AaaaperoBack.DTO
{
    public class AddJob
    {
        [Required]
        public int EmployerId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Remuneration { get; set; }

        [Required] public bool PremiumAdvertisement;
    }
}