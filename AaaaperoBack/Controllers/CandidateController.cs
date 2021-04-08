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
    public class CandidateController : ControllerBase
    {
        private readonly Context _context;

        public CandidateController(
            Context context,
            IUserService userService,
            IMapper mapper,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _context = context;
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
    }
}