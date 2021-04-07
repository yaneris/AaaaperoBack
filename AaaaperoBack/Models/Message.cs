using System;
using System.ComponentModel.DataAnnotations;

namespace AaaaperoBack.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public bool IsSenderEmployer { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }

        public Message()
        {
            CreatedDate = DateTime.UtcNow;
        }
    }
}