using SendGrid.Helpers.Mail;

namespace AaaaperoBack.DTO
{
    public class MessageDTO
    {
        public string Content { get; set; }
        public int ConversationId { get; set; }
    }
}