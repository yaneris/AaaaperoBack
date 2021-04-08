using System;
using System.ComponentModel.DataAnnotations;

namespace AaaaperoBack.DTO
{
    public class AddMessage
    {
        [Required]
        public int ConversationId { get; set; }
        [Required]
        public string Content { get; set; }

    }
}