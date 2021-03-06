using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AaaaperoBack.Data;
using Microsoft.EntityFrameworkCore;

using System.Linq;
using AaaaperoBack.Models;
using Microsoft.AspNetCore.Authorization;
using AaaaperoBack.Services;
using AutoMapper;
using AaaaperoBack.DTO;
using Microsoft.Extensions.Configuration;
using Advertisement = AaaaperoBack.Models.Job;

namespace AaaaperoBack.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobController : ControllerBase
    {
        private readonly Context _context;
        public JobController(Context context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Get all jobs created.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobDTO>>> GetJob()
        {
            var job = from jobs in _context.Job
                join title in _context.Job on jobs.Id equals title.Id
                select new JobDTO()
                {
                    Id = jobs.Id,
                    Title = jobs.Title,
                    Remuneration = jobs.Remuneration,
                    EmployerId = jobs.EmployerId,
                    Description = jobs.Description
                };

            return await job.ToListAsync();
        }

        /// <summary>
        /// Get a specific job advertisement.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<JobDTO> GetJob_byId(int id)
         {
             var job = from jobs in _context.Job
                 join title in _context.Job on jobs.Id equals title.Id
                 select new JobDTO()
                 {
                     Id = jobs.Id,
                     Title = jobs.Title,
                     Remuneration = jobs.Remuneration,
                     EmployerId = jobs.EmployerId,
                     Description = jobs.Description
                 };

             var job_byid = job.ToList().Find(x => x.Id == id);
             if (job_byid == null)
             {
                 return NotFound();
             }
 
             return job_byid;
         }
        
        /// <summary>
        /// Create a new job.
        /// </summary>
        /// <param name="jobDTO"></param>
        /// <returns></returns>

        [Authorize(Roles = Role.Admin + "," + Role.Employer + "," + Role.SuperUser)]
        [HttpPost]
        public async Task<ActionResult<AddJob>> Add_Job(AddJob jobDTO)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var job = new Job()
            {
                EmployerId = jobDTO.EmployerId,
                Title = jobDTO.Title,
                Description = jobDTO.Description,
                Remuneration = jobDTO.Remuneration,
            };
            await _context.Job.AddAsync(job);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction("GetJob", new { id = job.Id}, jobDTO);
        }

        /// <summary>
        /// Delete a specific job.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Employer + "," + Role.SuperUser)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Job>> Delete_Job(int id)
        {
            int loggedUserId = int.Parse(User.Identity.Name);
            var user = _context.User.Find(loggedUserId);
            var job = _context.Job.Find(id);
            if (job == null || job.EmployerId != user.Id)
            {
                return NotFound();
            }
            _context.Remove(job);
                await _context.SaveChangesAsync();
                return job;
        }
        
        /// <summary>
        /// Modify a specific job data.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="job"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Employer + "," + Role.SuperUser)]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update_Jobs(int id, JobDTO job)
        {
            int loggedUserId = int.Parse(User.Identity.Name);
            var user = _context.User.Find(loggedUserId);
            
            if(id != job.Id || !JobExists(id) || job.EmployerId != user.Id)
            {
                return BadRequest();
            }
            else 
            {
                var jobs = _context.Job.SingleOrDefault(x => x.Id == id);

                jobs.Id = job.Id;
                jobs.Description = job.Description;
                jobs.Title = job.Title;
                jobs.Remuneration = job.Remuneration;
                jobs.EmployerId = job.EmployerId;
                await _context.SaveChangesAsync();
                return NoContent();
            }
        }

        /// <summary>
        /// Allow the candidate connected to choose a job.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Candidate)]
        [HttpPost("ChooseJobs/{id}")]
        public async Task<ActionResult> ChooseJob(int id)
        {
            int loggedUserId = int.Parse(User.Identity.Name);
            var user = _context.User.Find(loggedUserId);

            if (loggedUserId != user.Id)
            {
                return BadRequest(new { message = "Access Denied" });
            }
            
            if (!JobExists(id))
            {
                return BadRequest();
            }
            else
            {
                var job = _context.Job.SingleOrDefault(x => x.Id == id);

                if (job.CandidateId != 0)
                {
                    return BadRequest(new { message = "Job already taken" });
                }
                else
                {
                    job.CandidateId = user.Id;
                }
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Get all the jobs the connected candidate is working on.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Candidate + "," + Role.Admin + "," + Role.SuperUser)]
        [HttpGet("GetMyJobs")]
        public List<Job> GetMyJobs()
        {
            int loggedUserId = int.Parse(User.Identity.Name);
            var user = _context.User.Find(loggedUserId);
            var jobs = _context.Job;
            var userJobs = new List<Job>();

            foreach(var job in jobs)
            {
                if(job.CandidateId == user.Id)
                {
                    userJobs.Add(job);
                }
            }
            return userJobs;
        }

        /// <summary>
        /// Add offer to the connected candidate.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Employer)]
        [HttpPost("AddOffer")]
        public Offer AddOffer( AddOffer offer)
        {
            int loggedUserId = int.Parse(User.Identity.Name);
            var user = _context.User.Find(loggedUserId);
            var jobs = _context.User.Find(offer.JobId);

            var of = new Offer
            {
                CandidateId = offer.CandidateId,
                EmployerId = user.Id,
                JobId = offer.JobId
            };

            _context.Offer.Add(of);
            _context.SaveChanges();

            return of;

        }

        /// <summary>
        /// Get all my offers from employers.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Candidate)]
        [HttpGet("GetMyOffers")]
        public ActionResult<OfferDTO> GetMyOffers()
        {
            int loggedUserId = int.Parse(User.Identity.Name);
            var user = _context.User.Find(loggedUserId);
            

            var offer = from offers in _context.Offer
            join jobs in _context.Job on offers.JobId equals jobs.Id
            select new OfferDTO
            {
                Id = offers.Id,
                EmployerId = offers.EmployerId,
                Job = jobs
            };

            return Ok(offer);
        }

        /// <summary>
        /// Accept or reject an offer.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Candidate)]
        [HttpPost("ManageOffer/{id}")]
        public async Task<ActionResult> ManageOffer(int offerId, AcceptOrRejectOffer offer)
        {
            int loggedUserId = int.Parse(User.Identity.Name);
            var user = _context.User.Find(loggedUserId);

            var offers = _context.Offer;
            var of = offers.SingleOrDefault(x => x.Id == offerId);

            var jobs = _context.Job;
             var job = jobs.SingleOrDefault(x => x.Id == of.JobId);

            if(offer.AcceptOrReject == "Accept" || offer.AcceptOrReject == "accept")
            {
                job.CandidateId = user.Id;
                _context.Remove(of);
                await _context.SaveChangesAsync();
            }
            if(offer.AcceptOrReject == "Reject" || offer.AcceptOrReject == "reject")
            {
               _context.Remove(of);
               await _context.SaveChangesAsync();
            }
            return NoContent();
        }

        /// <summary>
        /// Add skills to my candidate profile
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Candidate)]
        [HttpPost("AddSkillSet")]
        public ActionResult<SkillSet> AddSkillSet(SkillSetDTO skill)
        {
            int loggedUserId = int.Parse(User.Identity.Name);
            var user = _context.User.Find(loggedUserId);

            var skillToAdd = new SkillSet
            {
                Skill = skill.skill.ToString(),
                CandidateId = user.Id
            };

            _context.SkillSet.Add(skillToAdd);
            _context.SaveChanges();

            return Ok(skillToAdd);
        }

         

        //Check if the job exists in the database.
        private bool JobExists(int id)
        {
            return _context.Job.Find(id) != null;
        }
    }
}