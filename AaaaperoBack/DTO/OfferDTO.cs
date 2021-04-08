using AaaaperoBack.Models;

namespace AaaaperoBack.DTO
{
    public class OfferDTO
    {
        public int Id { get; set; }
        public int EmployerId { get; set; }
        public Job Job { get; set; }
    }
}