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
        /// Create a new conversation.
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
                    var employer = _context.Employer.SingleOrDefault(x => x.UserId == user.Id);
                    Conversation EmployerConversation = new Conversation();
                    EmployerConversation.EmployerId = employer.Id;
                    EmployerConversation.CandidateId = id;
                    _context.Conversation.Add(EmployerConversation);
                    _context.SaveChanges();
                    return Ok($"Conversation Succesfully created:{EmployerConversation.Id}");
                case Role.Candidate:
                    var candidate = _context.Candidate.SingleOrDefault(x => x.UserId == user.Id);
                    Conversation CandidateConversation = new Conversation();
                    CandidateConversation.CandidateId = candidate.Id;
                    CandidateConversation.EmployerId = id;
                    _context.Conversation.Add(CandidateConversation);
                    _context.SaveChanges();
                    return Ok($"Conversation Succesfully created:{CandidateConversation.Id}");
                default:
                    return BadRequest();
            }
        }
        
        /// <summary>
        /// Send a message.
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
                        ConversationId = messageDTO.ConversationId,
                        CreatedDate = DateTime.UtcNow
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
                        ConversationId = messageDTO.ConversationId,
                        CreatedDate = DateTime.UtcNow
                    };
                    _context.Message.Add(candidateMessage);
                    _context.SaveChanges();
                    return Ok(candidateMessage);
                
                default:
                    return BadRequest();
            }
        }
        
        /// <summary>
        /// Get all messages from a conversation.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<ConversationDTO> GetConversation(int id)
        {
            int loggedUserId = int.Parse(User.Identity.Name);
            var user = _context.User.Find(loggedUserId);
            
            /*if (user.Id != _context.Conversation.Find(id).CandidateId && user.Id != _context.Conversation.Find(id).EmployerId && user.Role != Role.Admin)
            {
                return BadRequest("Unauthorize");
            }*/

            var conversations = _context.Conversation;
            var conversation = conversations.SingleOrDefault(x => x.Id == id);
            if (conversation == null)
                return NotFound();

            var employer = _context.Employer.Find(conversation.EmployerId);
            var employerUser = _context.User.Find(employer.UserId);
            var candidate = _context.Candidate.Find(conversation.CandidateId);
            var candidateUser = _context.User.Find(candidate.UserId);
            var messages = _context.Message.ToList().FindAll(x => x.ConversationId == id);

            var conversationById = new ConversationDTO()
                {
                    Id = conversation.Id,
                    EmployerId = conversation.EmployerId,
                    EmployerName = employerUser.FirstName,
                    CandidateId = conversation.CandidateId,
                    CandidateName = candidateUser.FirstName,
                    Messages = messages
                };

            return conversationById;
        }
        
        /// <summary>
        /// Delete a conversation.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.SuperUser)]
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