namespace AaaaperoBack.DTO
{
    public class CandidateDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Skillset { get; set; }
        public string Description { get; set; }
        public bool Available { get; set; }
        
        public int TotalRate { get; set; }
        
        public int Count { get; set; }
    }
}