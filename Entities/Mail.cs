using System;

namespace Tracnghiem.Entities
{
    public class Mail
    {
        public Guid Id { get; set; }
        public string RecipientDisplayName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string RecipientEmail { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
