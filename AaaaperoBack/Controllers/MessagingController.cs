using System;
using System.Linq;
using System.Threading.Tasks;
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
        /// Start a new Conversation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("StartConversation")]
        public IActionResult StartConversation(int id)
        {
            int loggedUserId = int.Parse(User.Identity.Name);
            var user = _context.User.Find(loggedUserId);
            switch (user.Role)
            {
                case Role.Employer:
                    Conversation EmployerConversation = new Conversation();
                    EmployerConversation.EmployerId = user.Id;
                    EmployerConversation.CandidateId = id;
                    _context.Conversation.Add(EmployerConversation);
                    _context.SaveChanges();
                    return Ok($"Conversation Succesfully created:{EmployerConversation.Id}");
                case Role.Candidate:
                    Conversation CandidateConversation = new Conversation();
                    CandidateConversation.CandidateId = user.Id;
                    CandidateConversation.EmployerId = id;
                    _context.Conversation.Add(CandidateConversation);
                    _context.SaveChanges();
                    return Ok($"Conversation Succesfully created:{CandidateConversation.Id}");
                default:
                    return BadRequest();
            }
        }
        
        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="messageDTO"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("SendMessage")]
        public IActionResult SendMessage(AddMessage messageDTO)
        {
            int loggedUserId = int.Parse(User.Identity.Name);
            var user = _context.User.Find(loggedUserId);

            switch (user.Role)
            {
                case Role.Employer:
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
                
                case Role.Candidate:
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
        /// <param name="GetConversation"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<ConversationDTO> GetConversation(int id)
        {
            int loggedUserId = int.Parse(User.Identity.Name);
            var user = _context.User.Find(loggedUserId);
            
            if (user.Id != _context.Conversation.Find(id).CandidateId && user.Id != _context.Conversation.Find(id).EmployerId && user.Role != Role.Admin)
            {
                return BadRequest("Unauthorize");
            }
            var conversations = from conversation in _context.Conversation
                join Message in _context.Message on conversation.Id equals Message.ConversationId
                join candidate in _context.User on conversation.CandidateId equals candidate.Id
                select new ConversationDTO()
                {
                    Id = id,
                    EmployerId = conversation.EmployerId,
                    EmployerName = user.FirstName,
                    CandidateId = conversation.CandidateId,
                    CandidateName = candidate.FirstName,
                    Messages = conversation.Messages
                };

            var conversation_byId = conversations.ToList().Find(x => x.Id == id);

            if (conversation_byId == null)
            {
                return BadRequest();
            }

            return conversation_byId;
        }
        
        /// <summary>
        /// Delete a conversation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpDelete("DeleteConversation{id}")]
        public IActionResult Delete_Conversation(int id)
        {
            var conversation = _context.Conversation.Find(id);

            if(conversation == null)
            {
                return NotFound();
            }
            else 
            {
                _context.Remove(conversation);
                return Ok("Successfully deleted !");
            }
        }
    }
}