using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace AaaaperoBack.DTO
{
    public class RegisterModel
    {
        ///<summary>
        ///
        ///</summary>
        ///<example>First name</example>
        [Required]
        public string FirstName { get; set; }

        ///<summary>
        ///
        ///</summary>
        ///<example>Last name</example>
        [Required]
        public string LastName { get; set; }

        ///<summary>
        ///
        ///</summary>
        ///<example>Candidate | Employer</example>
        [Required]
        public string Role { get; set; }

        ///<summary>
        ///
        ///</summary>
        ///<example>Username</example>
        [Required]
        public string Username { get; set; }

        ///<summary>
        ///
        ///</summary>
        ///<example>Password</example>
        [Required]
        public string Password { get; set; }
        
        ///<summary>
        ///
        ///</summary>
        ///<example>Email</example>
        [Required]
        public string Email { get; set; }
    }
}