using System;
using System.Collections.Generic;

namespace CRUDService.Models
{
    public partial class Conversations
    {
        public Conversations()
        {
            Messages = new HashSet<Messages>();
            UsersConversations = new HashSet<UsersConversations>();
        }

        public Guid ConversationId { get; set; }

        public virtual ICollection<Messages> Messages { get; set; }
        public virtual ICollection<UsersConversations> UsersConversations { get; set; }
    }
}
