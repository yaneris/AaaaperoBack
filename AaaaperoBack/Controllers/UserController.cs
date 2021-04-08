using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

using AutoMapper;

using AaaaperoBack.Data;
using AaaaperoBack.DTO;
using AaaaperoBack.Helpers;
using AaaaperoBack.Models;
using AaaaperoBack.Services;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.Threading.Tasks;

namespace AaaaperoBack.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        public IConfiguration Configuration;
        private readonly Context _context;
        private readonly IEmailService _emailService;

        public UserController(
            Context context,
            IUserService userService,
            IMapper mapper,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _context = context;
            _userService = userService;
            _mapper = mapper;
            Configuration = configuration;
            _emailService = emailService;
        }

        ///<summary>
        /// Create a new user
        ///</summary>
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            string[] roles = { "Admin", "SuperUser", "Candidate", "Employer" };
            if (!Array.Exists(roles, s => s == model.Role))
                return BadRequest(new { message = "Unknown role" });
            // map model to entity
            var user = _mapper.Map<User>(model);
            user.IsEnabled = true;
            try
            {
                // create user
                _userService.Create(user, model.Password);
                switch (user.Role)
                {
                    case Role.Candidate:
                        if (!ModelState.IsValid)
                        {
                            return BadRequest(ModelState);
                        }

                        var candidate = new Candidate()
                        {
                            UserId = user.Id,
                            Skillset = "",
                            Available = true,
                            Description = "",

                        };
                        _context.Candidate.Add(candidate);
                        _context.SaveChanges();
                        break;
                    case Role.Employer:
                        if (!ModelState.IsValid)
                        {
                            return BadRequest(ModelState);
                        }

                        var employer = new Employer()
                        {
                            UserId = user.Id,
                            Description = "",
                        };
                        _context.Employer.Add(employer);
                        _context.SaveChanges();
                        break;
                }
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(Configuration["Secret"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.Role ?? "null")
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                // return basic user info and authentication token
                return Ok(new
                {
                    Id = user.Id,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Token = tokenString,
                    Code = tokenString
                });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Authentication method.
        /// </summary>
        /// <param name="model"></param>
        ///         /// <remarks>
        /// Sample request:
        ///
        ///     POST /user/register
        ///     {
        ///         "username": "Username",
        ///         "password": "Password"
        ///     }
        ///
        /// </remarks>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public IActionResult Authenticate(AuthenticateModel model)
        {
            var user = _userService.Authenticate(model.Username, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            if (!user.IsEnabled)
            {
                return BadRequest(new { message = "Your Account has been disabled" });
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Configuration["Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role ?? "null")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info and authentication token
            return Ok(new
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Token = tokenString
            });
        }

        /// <summary>
        /// Change the AccessLevel of a user.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpPost("Accesslevel/{id}")]
        public IActionResult ChangeAccess(int id, UpdateRoleDTO model)
        {
            // You should check if the user exists or not and then check what is their current access level.
            _context.User.Find(id).Role = model.Role;
            _context.SaveChanges();
            return Ok("User Access Level has been updated!");
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet("Users")]
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetAll();
            var model = _mapper.Map<IList<UserModel>>(users);
            return Ok(model);
        }

        //[Authorize(Roles = AccessLevel.Admin + "," + AccessLevel.Candidate + "," + AccessLevel.Employer)]
        [AllowAnonymous]
        [HttpPut("Myprofile")]
        public async Task<ActionResult> GetMyProfile(UpdateUserDTO model)
        {
            int loggedUserId = int.Parse(User.Identity.Name);
            var user = _context.User.Find(loggedUserId);

            if (loggedUserId != user.Id)
            {
                return BadRequest(new { message = "Access Denied" });
            }

            switch (user.Role)
            {
                case Role.Candidate:
                    var candidate = new CandidateDTO
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Username = model.Username,
                        Email = model.Email,
                        Skillset = model.Skillset,
                        Available = model.Available,
                        Description = model.Description
                    };
                    var candidateDB = _context.User.Find(loggedUserId);
                    candidateDB.FirstName = candidate.FirstName;
                    candidateDB.LastName = candidate.LastName;
                    candidateDB.Username = candidate.Username;
                    candidateDB.Email = candidate.Email;
                    var candidateDBcan = _context.Candidate.SingleOrDefault(x => x.UserId == loggedUserId);
                    candidateDBcan.Skillset = candidate.Skillset;
                    candidateDBcan.Available = candidate.Available;
                    candidateDBcan.Description = candidate.Description;
                    await _context.SaveChangesAsync();
                    return Ok(candidate);
                case Role.Employer:
                    var employer = new EmployerDTO
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Username = model.Username,
                        Email = model.Email,
                        Description = model.Description

                    };
                    var employerDB = _context.User.Find(loggedUserId);
                    employerDB.FirstName = employer.FirstName;
                    employerDB.LastName = employer.LastName;
                    employerDB.Username = employer.Username;
                    employerDB.Email = employer.Email;
                    var employerDBemp = _context.Employer.SingleOrDefault(x => x.UserId == loggedUserId);
                    employerDBemp.Description = employer.Description;
                    await _context.SaveChangesAsync();
                    return Ok(employer);
                case Role.Admin:
                    var admin = new UpdateAdminDTO
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Username = model.Username,
                        Email = model.Email
                    };
                    var adminDB = _context.User.Find(loggedUserId);
                    adminDB.FirstName = admin.FirstName;
                    adminDB.LastName = admin.LastName;
                    adminDB.Username = admin.Username;
                    adminDB.Email = admin.Username;
                    await _context.SaveChangesAsync();
                    return Ok(admin);
            }
            return NoContent();
        }

        /// <summary>
        /// Restore the given deleted user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpPost("Restore_Account/{id}")]
        public IActionResult Restore(int id)
        {
            var user = _userService.GetById(id);
            _context.User.Find(id).IsEnabled = true;
            _context.SaveChanges();
            return Ok($"User {user.Username} account has been succefully enabled");
        }

        /// <summary>
        /// Disable the given admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.SuperUser)]
        [HttpPost("Disable_admin_Account/{id}")]
        public IActionResult AdminDisable(int id)
        {
            var user = _userService.GetById(id);
            _context.User.Find(id).IsEnabled = false;
            _context.SaveChanges();
            return Ok($"{user.Username} account has been succefully disabled");
        }

        [Authorize(Roles = Role.Employer + "," + Role.Admin)]
        [HttpGet("Candidates")]
        public List<CandidateDTO> GetCandidates()
        {
            var users = _userService.GetAll();
            var candidates = new List<CandidateDTO>();
            foreach (var user in users)
            {
                if (user.Role == "Candidate")
                {
                    var canDB = _context.Candidate.SingleOrDefault(x => x.UserId == user.Id);
                    var candidate = new CandidateDTO
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Username = user.Username,
                        Email = user.Email,
                        Skillset = canDB.Skillset,
                        Available = canDB.Available,
                        Description = canDB.Description
                        
                    };

                    candidates.Add(candidate);
                }
            }
            return (candidates);
        }

        [Authorize(Roles = Role.Candidate + "," + Role.Admin)]
        [HttpGet("Employers")]
        public List<EmployerDTO> GetEmployers()
        {
            var users = _userService.GetAll();
            var employers = new List<EmployerDTO>();
            foreach (var user in users)
            {
                if (user.Role == "Employer")
                {
                    var empDB = _context.Employer.SingleOrDefault(x => x.UserId == user.Id);
                    var employer = new EmployerDTO()
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Username = user.Username,
                        Email = user.Email,
                        Description = empDB.Description
                        //ADD JOB !!!
                    };

                    employers.Add(employer);
                }
            }
            return(employers);
        }
            

    /// <summary>
    /// Disable the given user
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = Role.Admin)]
    [HttpPost("Disable_normalAccount/{id}")]
    public IActionResult Disable(int id)
    {
        var user = _userService.GetById(id);
        if (user.Role != Role.Admin)
        {
            _context.User.Find(id).IsEnabled = false;
            _context.SaveChanges();
            return Ok($"{user.Username} account has been succefully disabled");
        }
        return BadRequest($"{user.Username} is an Admin !");
    }

    /// <summary>
    /// Hard Delete the given user
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = Role.Admin)]
    [HttpDelete("Hard_Delete_Account/{id}")]
    public IActionResult HardDelete(int id)
    {
        var user = _userService.GetById(id);
        _userService.Delete(id);
        return Ok($"{user.Username} account has been succefully deleted from the database");
    }
    //to send an email
    [AllowAnonymous]
    [HttpPost("forgotpassword")]
    public IActionResult ForgotPassword(ForgotPassword model)
    {
        return Ok(_userService.ForgotPassword(model.Username));
    }
    
    //to reset 
    [AllowAnonymous]
    [HttpPost("ResetPassword")]
    public IActionResult ResetPassword(ResetPasswordDTO model)
    {
        return Ok(_userService.ResetPassword(model.Username,model.EmailToken,model.NewPassword,model.ConfirmNewPassword));
    }
    //only for admin
    [Authorize(Roles = Role.Admin)]
    [HttpPost("email")]
    public async Task<IActionResult> SendEmail(SendEmailDTO model)
    {
        var emails = new List<string>();
        foreach (var item in model.emails)
        {
            emails.Add(item);
        }
        var response = await _emailService.SendEmailAsync(emails, model.Subject, model.Message);
        if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
        {
            return Ok("Email sent " + response.StatusCode);
        }
        else
        {
            return BadRequest("Email sending failed " + response.StatusCode);
        }
    }
}
}