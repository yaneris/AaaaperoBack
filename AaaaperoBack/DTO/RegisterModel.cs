using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace AaaaperoBack.DTO
{
    public class RegisterModel
    {
        ///<summary>
        ///
        ///</summary>
        ///<example> First name </example>
        [Required]
        public string FirstName { get; set; }

        ///<summary>
        ///
        ///</summary>
        ///<example> Last name </example>
        [Required]
        public string LastName { get; set; }

        ///<summary>
        ///
        ///</summary>
        ///<example> Candidate or Employer </example>
        [Required]
        public string Field { get; set; }

        ///<summary>
        ///
        ///</summary>
        ///<example> Username </example>
        [Required]
        public string Username { get; set; }

        ///<summary>
        ///
        ///</summary>
        ///<example> Password </example>
        [Required]
        public string Password { get; set; }
    }
}