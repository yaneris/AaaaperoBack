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
    public class AdminController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        public IConfiguration Configuration;
        private readonly Context _context;
        private readonly IEmailService _emailService;

        public AdminController(
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

        /// <summary>
        /// Display all users
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpGet("Users")]
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetAll();
            var model = _mapper.Map<IList<UserModel>>(users);
            return Ok(model);
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
        /// Display Candidates
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Employer)]
        [HttpGet("Candidates")]
        public ActionResult<CandidateDTO> GetCandidates()
        {
            var candidates = from candidate in _context.Candidate
                join user in _context.User on candidate.UserId equals user.Id
                select new CandidateDTO
                {
                    Id = candidate.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    Email = user.Email,
                    Skillset = candidate.Skillset,
                    Available = candidate.Available,
                    Description = candidate.Description
                };
            return Ok(candidates);
        }
        
        /// <summary>
        /// Display the given Candidate
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Employer)]
        [HttpGet("Candidate/{id}")]
        public ActionResult<CandidateDTO> GetCandidate_byId(int id)
        {
            var candidates = _context.Candidate;
            var candidate = candidates.SingleOrDefault(x => x.Id == id);
            if (candidate == null)
            {
                return NotFound();
            }
            var user = _context.User.Find(candidate.UserId);

            var candidateById = new CandidateDTO()
            {
                Id = candidate.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Description = candidate.Description,
                Skillset = candidate.Skillset,
                Available = candidate.Available
            };
            
            return candidateById;
        }

        /// <summary>
        /// Display Employers
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Candidate + "," + Role.Admin)]
        [HttpGet("Employers")]
        public ActionResult<EmployersDTO> GetEmployers()
        {
            var employers = from employer in _context.Employer
                join user in _context.User on employer.UserId equals user.Id
                select new EmployersDTO
                {
                    Id = employer.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    Email = user.Email,
                    Description = employer.Description
                };
            return Ok(employers);
        }
        
        /// <summary>
        /// Display the given Employer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Candidate)]
        [HttpGet("Employer/{id}")]
        public ActionResult<EmployerDTO> GetEmployer_byId(int id)
        {
            var employers = _context.Employer;
            var employer = employers.SingleOrDefault(x => x.Id == id);
            if (employer == null)
            {
                return NotFound();
            }
            var user = _context.User.Find(employer.UserId);
            var jobs = _context.Job.ToList().FindAll(x => x.EmployerId == id).OrderBy(x => !x.PremiumAdvertisement).ToList();

            var employerById = new EmployerDTO()
            {
                Id = employer.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Description = employer.Description,
                Jobs = jobs,
                Email = user.Email
            };
            
            return employerById;
        }
            

        /// <summary>
        /// Disable the given User
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
    }
}