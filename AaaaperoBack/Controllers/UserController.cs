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

namespace AaaaperoBack.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        public IConfiguration Configuration;
        private readonly Context _context;
        private readonly IEmailService _emailService;

        public UserController(
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
        
        ///<summary>
        /// Create a new user
        ///</summary>
        [AllowAnonymous]
        [HttpPost("Register")]
        
        public IActionResult Register([FromBody]RegisterModel model)
        {
            // map model to entity
            var user = _mapper.Map<User>(model);

            try
            {
                // create user
                _userService.Create(user, model.Password);
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(Configuration["Secret"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.AccessLevel ?? "null")
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                // return basic user info and authentication token
                return Ok(new
                {
                    Id = user.Id,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = tokenString
                });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
        
        /// <summary>
        /// Authentication method.
        /// </summary>
        /// <param name="model"></param>
        ///         /// <remarks>
        /// Sample request:
        ///
        ///     POST /user/register
        ///     {
        ///         "username": "Username",
        ///         "password": "Password"
        ///     }
        ///
        /// </remarks>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public IActionResult Authenticate(AuthenticateModel model)
        {
            var user = _userService.Authenticate(model.Username, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            if (!user.IsEnabled)
            {
                return BadRequest(new { message = "Your Account has been disabled" });
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Configuration["Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.AccessLevel ?? "null")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info and authentication token
            return Ok(new
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = tokenString
            });
        }
        
        /// <summary>
        /// Change the AccessLevel of a user.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccessLevel.Admin)]
        [HttpPost("Accesslevel/{id}")]
        public IActionResult ChangeAccess(int id, UpdateAccessLevelDTO model)
        {
            // You should check if the user exists or not and then check what is their current access level. As well as you need to create an enum or make sure that user does not pass any 
            // value except the allowed values which are: NULL, Admin, Support, Student Lead
            _context.User.Find(id).AccessLevel = model.AccessLevel;
            _context.SaveChanges();
            return Ok("User Access Level has been updated!");
        }

        /// <summary>
        /// Display all the users
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = AccessLevel.Admin)]
        [HttpGet]
        public IActionResult GetAll()
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
        [Authorize(Roles = AccessLevel.Admin)]
        [HttpPost("Restore_Account/{id}")]
        public IActionResult Restore(int id)
        {
            var user = _userService.GetById(id);
            _context.User.Find(id).IsEnabled = true;
            _context.SaveChanges();
            return Ok($"User {user.Username} account has been succefully enabled");
        }
        
        /// <summary>
        /// Disable the given admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = AccessLevel.SuperUser)]
        [HttpPost("Disable_admin_Account/{id}")]
        public IActionResult AdminDisable(int id)
        {
            var user = _userService.GetById(id);
            _context.User.Find(id).IsEnabled = false;
            _context.SaveChanges();
            return Ok($"{user.Username} account has been succefully disabled");
        }
        
        /// <summary>
        /// Disable the given user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = AccessLevel.Admin)]
        [HttpPost("Disable_normalAccount/{id}")]
        public IActionResult Disable(int id)
        {
            var user = _userService.GetById(id);
            if (user.AccessLevel != AccessLevel.Admin)
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
        [Authorize(Roles = AccessLevel.Admin)]
        [HttpDelete("Hard_Delete_Account/{id}")]
        public IActionResult HardDelete(int id)
        {
            var user = _userService.GetById(id);
            _userService.Delete(id);
            return Ok($"{user.Username} account has been succefully deleted from the database");
        }
        
    }
}