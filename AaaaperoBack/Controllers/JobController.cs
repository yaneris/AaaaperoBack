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
        /// get all job created
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
                    PremiumAdvertisement = jobs.PremiumAdvertisement,
                    Description = jobs.Description
                };

            return await job.ToListAsync();
        }

        /// <summary>
        /// get a special user, we need to put an id in param
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
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
                     PremiumAdvertisement = jobs.PremiumAdvertisement,
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
        /// method in order to create a new job
        /// </summary>
        /// <param name="jobDTO"></param>
        /// <returns></returns>

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<AddJob>> Add_Job(AddJob jobDTO)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var job = new Job()
            {
                Id = jobDTO.Id,
                EmployerId = jobDTO.EmployerId,
                Title = jobDTO.Title,
                Description = jobDTO.Description,
                Remuneration = jobDTO.Remuneration,
                PremiumAdvertisement = jobDTO.PremiumAdvertisement
            };
            await _context.Job.AddAsync(job);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction("GetJob", new { id = job.Id}, jobDTO);
        }

        /// <summary>
        /// Method which delete a job take one param id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Job>> Delete_Job(int id)
        {
            var job = _context.Job.Find(id);
            if (job == null)
            {
                return NotFound();
            }
            _context.Remove(job);
                await _context.SaveChangesAsync();
                return job;
        }
        
        /// <summary>
        /// Modify an existing job by taking an id as parameter
        /// </summary>
        /// <param name="id"></param>
        /// <param name="job"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update_Jobs(int id, JobDTO job)
        {
            if(id != job.Id || !JobExists(id))
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
                jobs.PremiumAdvertisement = job.PremiumAdvertisement;
                await _context.SaveChangesAsync();
                return NoContent();
            }
        }

        private bool JobExists(int id)
        {
            return _context.Job.Find(id) != null;
        }
    }
}