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
using Advertisement = AaaaperoBack.Models.Advertisement;

namespace AaaaperoBack.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AdvertisementController : ControllerBase
    {
        private readonly Context _context;
        public AdvertisementController(Context context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdvertisementDTO>>> GetAdvertisement()
        {
            var advertisement = from advertisements in _context.Advertisement
                join title in _context.Advertisement on advertisements.Id equals title.Id
                select new AdvertisementDTO
                {
                    Id = advertisements.Id,
                    Title = advertisements.Title,
                    Creation = advertisements.Creation,
                    Description = advertisements.Description,
                    IsOpen = advertisements.IsOpen
                };

            return await advertisement.ToListAsync();
        }

        /*[HttpGet("{id}")]
        public ActionResult<AdvertisementDTO> GetAdvertisement_byId(int id)
         {
             var advertisement = from advertisements in _context.Advertisement
                 join title in _context.Advertisement on advertisements.Id equals title.Id
                 select new AdvertisementDTO
                 {
                     Id = advertisements.Id,
                     Title = advertisements.Title,
                     Creation = advertisements.Creation,
                     Description = advertisements.Description,
                     IsOpen = advertisements.IsOpen
                 };
             var advertisement_byid = advertisement.ToList().Find(x => x.Id == id);
             if (advertisement_byid == null)
             {
                 return NotFound();
             }
 
             return advertisement_byid;
         }
         */
     

    }
}