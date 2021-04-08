namespace AaaaperoBack.DTO
{
    public class ResetPasswordDTO
    {
       
        public string Username { get; set; }
        public string EmailToken { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}