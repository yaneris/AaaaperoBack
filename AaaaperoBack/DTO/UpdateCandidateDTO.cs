namespace AaaaperoBack.DTO
{
    public class UpdateCandidateDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string email { get; set; }
        public string skillset { get; set; }
        public bool availability { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}