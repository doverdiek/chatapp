using System;
using System.Collections.Generic;
using System.Text;

namespace DTOModels
{
    public class UsersConversationsDTO
    {
        public Guid? UserId { get; set; }
        public Guid? ConversationId { get; set; }

        public  ConversationsDTO Conversation { get; set; }
        public  UsersDTO User { get; set; }
    }
}
