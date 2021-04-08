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

    }
}