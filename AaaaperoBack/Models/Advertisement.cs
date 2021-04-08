using System.ComponentModel.DataAnnotations;

namespace AaaaperoBack.Models
{
    public class Advertisement
    {
        [Key]
        public int AdId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
    }
}