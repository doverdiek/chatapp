using System;
using System.Collections.Generic;
using System.Text;

namespace DTOModels
{
    public class ConversationsDTO
    {
        public Guid ConversationId { get; set; }

        public  ICollection<MessagesDTO> Messages { get; set; }
        public  ICollection<UsersConversationsDTO> UsersConversations { get; set; }
    }
}
