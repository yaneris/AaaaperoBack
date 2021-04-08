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
        /// Change the Role of a user.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.SuperUser)]
        [HttpPost("Role/{id}")]
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
        [Authorize(Roles = Role.Admin + "," + Role.SuperUser)]
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
        [Authorize(Roles = Role.Admin + "," + Role.SuperUser)]
        [HttpPost("Restore_Account/{id}")]
        public IActionResult Restore(int id)
        {
            var user = _userService.GetById(id);
            _context.User.Find(id).IsEnabled = true;
            _context.SaveChanges();
            return Ok($"User {user.Username} account has been succefully enabled");
        }
        
        /// <summary>
        /// Disable the given User
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.SuperUser)]
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
        [Authorize(Roles = Role.Admin + "," + Role.SuperUser)]
        [HttpDelete("Hard_Delete_Account/{id}")]
        public IActionResult HardDelete(int id)
        {
            var user = _userService.GetById(id);
            _userService.Delete(id);
            return Ok($"{user.Username} account has been succefully deleted from the database");
        } 
    }
}