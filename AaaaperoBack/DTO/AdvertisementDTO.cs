using System;
using System.ComponentModel.DataAnnotations;

namespace AaaaperoBack.DTO
{
    public class AdvertisementDTO
    {
        [Key] public int Id { get; set; }
        [Required] public string Title { get; set; }
        [Required] public string Description { get; set; }

        public DateTime Creation { get; set; }
        
        public bool IsOpen { get; set; }
    }
}