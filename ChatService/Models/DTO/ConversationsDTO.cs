using System;
using System.Collections.Generic;
using System.Text;

namespace DTOModels
{
    public class ConversationsDTO
    {
        public Guid? ConversationId { get; set; }

        public  List<MessagesDTO> Messages { get; set; }
        public  List<UsersConversationsDTO> UsersConversations { get; set; }
    }
}
