using System;
using System.ComponentModel.DataAnnotations;

namespace AaaaperoBack.Models
{
    public class Advertisement
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        
        public DateTime Creation { get; set; }
        
        public bool IsOpen { get; set; }
    }
}