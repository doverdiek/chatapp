using System;
using System.Collections.Generic;

namespace CRUDService.Models
{
    public partial class UsersConversations
    {
        public Guid UserId { get; set; }
        public Guid ConversationId { get; set; }

        public virtual Conversations Conversation { get; set; }
        public virtual Users User { get; set; }
    }
}
