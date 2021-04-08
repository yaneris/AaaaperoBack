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
    public class EmployersController : ControllerBase
    {
        private readonly Context _context;

        public EmployersController(
            Context context )
        {
            _context = context;
        }
        
        /// <summary>
        /// Display Employers
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet()]
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

            var employersList = _context.User.ToList().OrderBy(x => x.Premium == true);
            
            return Ok(employers);
        }
        
        /// <summary>
        /// Display the given Employer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Candidate)]
        [HttpGet("{id}")]
        public ActionResult<EmployerDTO> GetEmployer_byId(int id)
        {
            var employers = _context.Employer;
            var employer = employers.SingleOrDefault(x => x.Id == id);
            if (employer == null)
            {
                return NotFound();
            }
            var user = _context.User.Find(employer.UserId);
            var jobs = _context.Job.ToList().FindAll(x => x.EmployerId == id).ToList();

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
        /// Allow an employer to pay a candidate
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Employer)]
        [HttpPost("PayCandidate")]
        public async Task<ActionResult> PayCandidate(int jobId)
        {
            int loggedUserId = int.Parse(User.Identity.Name);
            var employer = _context.Employer.Find(loggedUserId);
            var job = _context.Job.Find(jobId);

            if (job.CandidateId != 0)
            {
                if (job == null)
                {
                    return NotFound();
                }

                _context.Remove(job);
                await _context.SaveChangesAsync();

                return Ok("The payement has been sent.");
            }
            else
            {
                return Ok("No one is assigned to this job.");
            }




        }

    }
}