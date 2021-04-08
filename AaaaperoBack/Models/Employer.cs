using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AaaaperoBack.Models
{
    public class Employer
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
    }
}