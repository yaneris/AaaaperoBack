namespace AaaaperoBack.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string AccessLevel { get; set; }
        
        public string Field { get; set; }
        public string Token { get; set; }
    }
}