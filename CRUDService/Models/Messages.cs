using System;
using System.Collections.Generic;

namespace CRUDService.Models
{
    public partial class Messages
    {
        public Guid MessageId { get; set; }
        public string Message { get; set; }
        public Guid ConversationId { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid UserId { get; set; }

        public virtual Conversations Conversation { get; set; }
        public virtual Users User { get; set; }
    }
}
