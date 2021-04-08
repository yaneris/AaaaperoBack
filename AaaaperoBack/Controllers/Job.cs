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
    public class AdvertisementController : ControllerBase
    {
        private readonly Context _context;
        public AdvertisementController(Context context)
        {
            _context = context;
        }
        
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobDTO>>> GetAdvertisement()
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
        public ActionResult<JobDTO> GetAdvertisement_byId(int id)
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
     

    }
}