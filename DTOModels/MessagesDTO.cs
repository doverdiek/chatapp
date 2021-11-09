using System;
using System.Collections.Generic;
using System.Text;

namespace DTOModels
{
    public class MessagesDTO
    {
        public Guid MessageId { get; set; }
        public string Message { get; set; }
        public Guid ConversationId { get; set; }
        public Guid UserId { get; set; }

        public  ConversationsDTO Conversation { get; set; }
        public  UsersDTO User { get; set; }
    }
}
