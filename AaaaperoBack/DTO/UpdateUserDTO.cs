namespace AaaaperoBack.DTO
{
    public class UpdateUserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Skillset { get; set; }
        public string Description { get; set; }
        public bool Available { get; set; }
        /*public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }*/
    }
}