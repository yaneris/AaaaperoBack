using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AaaaperoBack.Data;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AaaaperoBack.Models;
using Microsoft.AspNetCore.Authorization;
using AaaaperoBack.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
            _context.Job.AddAsync(job);
            _context.SaveChanges();
            
            return CreatedAtAction("GetJob", new { id = job.Id}, jobDTO);
        }

     

    }
}