using System.ComponentModel.DataAnnotations;
namespace AaaaperoBack.Models
{
    public class ForgotPassword
    {
        [Required]
        public string Username { get; set; }
    }
}