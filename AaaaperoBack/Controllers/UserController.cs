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
        /// Display profile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("UpdateMyprofile")]
        public async Task<ActionResult> UpdateMyProfile(UpdateUserDTO model)
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
                    var employer = new EmployersDTO
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
        /// Display profile
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetMyprofile")]
        public ActionResult GetMyProfile()
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
                    var candidateDB = _context.User.Find(loggedUserId);
                    var candidateDBcan = _context.Candidate.SingleOrDefault(x => x.UserId == loggedUserId);
                    var candidate = new CandidateDTO
                    {
                        FirstName = candidateDB.FirstName,
                        LastName = candidateDB.LastName,
                        Username = candidateDB.Username,
                        Email = candidateDB.Email,
                        Skillset = candidateDBcan.Skillset,
                        Available = candidateDBcan.Available,
                        Description = candidateDBcan.Description
                    };
                    return Ok(candidate);
                case Role.Employer:
                    var employerDB = _context.User.Find(loggedUserId);
                    var employerDBemp = _context.Employer.SingleOrDefault(x => x.UserId == loggedUserId);
                    var employer = new EmployersDTO
                    {
                        FirstName = employerDB.FirstName,
                        LastName = employerDB.LastName,
                        Username = employerDB.Username,
                        Email = employerDB.Email,
                        Description = employerDBemp.Description

                    };
                    return Ok(employer);
                case Role.Admin:
                var adminDB = _context.User.Find(loggedUserId);
                    var admin = new UpdateAdminDTO
                    {
                        FirstName = adminDB.FirstName,
                        LastName = adminDB.LastName,
                        Username = adminDB.Username,
                        Email = adminDB.Email
                    };
                return Ok(admin);
            }
            return NoContent();
        }

        [Authorize]
        [HttpPost("Bump")]
        public async Task<IActionResult> Bump()
        {
            int loggedUserId = int.Parse(User.Identity.Name);
            var user = _context.User.Find(loggedUserId);

            user.Premium = true;
            await _context.SaveChangesAsync();

            return Ok("Account successfully bumped !");
        }
    }
}