using System;
using AaaaperoBack.Data;
using AaaaperoBack.DTO;
using AaaaperoBack.Models;
using AaaaperoBack.Services;
using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AaaaperoBack.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MessagingController : ControllerBase
    {
        private readonly Context _context;
        
        public MessagingController(
            Context context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="messageDTO"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public IActionResult SendMessage(AddMessage messageDTO)
        {
            int loggedUserId = int.Parse(User.Identity.Name);
            var user = _context.User.Find(loggedUserId);

            switch (user.Role)
            {
                case AccessLevel.Employer:
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    var employerMessage = new Message()
                    {
                        IsSenderEmployer = true,
                        Content = messageDTO.Content,
                        ConversationId = messageDTO.ConversationId
                    };
                    _context.Message.Add(employerMessage);
                    _context.SaveChanges();
                    return Ok(employerMessage);
                
                case AccessLevel.Candidate:
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    var candidateMessage = new Message()
                    {
                        IsSenderEmployer = false,
                        Content = messageDTO.Content,
                        ConversationId = messageDTO.ConversationId
                    };
                    _context.Message.Add(candidateMessage);
                    _context.SaveChanges();
                    return Ok(candidateMessage);
                
                default:
                    return BadRequest();
            }
        }
        
        /// <summary>
        /// Get all messages from a conversation
        /// </summary>
        /// <param name="messageDTO"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public IActionResult Get(AddMessage messageDTO)
        {
            int loggedUserId = int.Parse(User.Identity.Name);
            var user = _context.User.Find(loggedUserId);

            switch (user.Role)
            {
                case AccessLevel.Employer:
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    var employerMessage = new Message()
                    {
                        IsSenderEmployer = true,
                        Content = messageDTO.Content,
                        ConversationId = messageDTO.ConversationId
                    };
                    _context.Message.Add(employerMessage);
                    _context.SaveChanges();
                    return Ok(employerMessage);
                
                case AccessLevel.Candidate:
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    var candidateMessage = new Message()
                    {
                        IsSenderEmployer = false,
                        Content = messageDTO.Content,
                        ConversationId = messageDTO.ConversationId
                    };
                    _context.Message.Add(candidateMessage);
                    _context.SaveChanges();
                    return Ok(candidateMessage);
                
                default:
                    return BadRequest();
            }
        }
    }
}