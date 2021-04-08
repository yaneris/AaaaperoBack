using System.ComponentModel.DataAnnotations;

namespace AaaaperoBack.Models
{
    public class Candidate
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Skillset { get; set; }
        public string Description { get; set; }
        public bool Available { get; set; }
        
        public int TotalRate { get; set; }
        
        public int Count { get; set; }
    }
}