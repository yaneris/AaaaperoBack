using System;
using System.ComponentModel.DataAnnotations;

namespace AaaaperoBack.Models
{
    // Message attached to a Conversation. Sender is defined by `IsSenderEmployer` prop
    public class Message
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ConversationId { get; set; }
        [Required]
        public bool IsSenderEmployer { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }

        public Message()
        {
            CreatedDate = DateTime.UtcNow;
        }
    }
}