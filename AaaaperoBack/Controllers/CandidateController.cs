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
        /// Display a specific candidate.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
                
        [Authorize]
        [HttpGet("{id}")]
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
        /// Display all candidates.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
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

                var candidatesList = _context.User.ToList().OrderBy(x => x.Premium == true);
            return Ok(candidates);
        }

        /// <summary>
        /// Method which rate the given employer from his job advertisement.
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        [HttpPost("/rateEmployer")]
        [Authorize(Roles = Role.Admin + "," + Role.Candidate + "," + Role.SuperUser)]
        public async Task<ActionResult> RatingEmployer(int jobId, int rate)
        {
            int loggedUserId = int.Parse(User.Identity.Name);
            var candidate = _context.Candidate.Find(loggedUserId);
            var job = _context.Job.Find(jobId);
            

            if (job.EmployerId != 0)
            {
                if (candidate == null)
                {
                    
                    return NotFound();
                }
                var employer = _context.Employer.Find(job.EmployerId);
                employer.Count++;
                employer.TotalRate = (employer.TotalRate + rate) / employer.Count;
                
                
                _context.Remove(job);
                await _context.SaveChangesAsync();

                return Ok("Thanks for rating");
            }
            else
            {
                return Ok("Can't find employer for this job");
            }
        }
    }
}