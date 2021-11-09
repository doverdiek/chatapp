using System;
using System.Collections.Generic;

namespace DTOModels
{
    public class UsersDTO
    {
        public Guid? UserId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Picture { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public List<MessagesDTO> Messages { get; set; }
        public List<UsersConversationsDTO> UsersConversations { get; set; }
    }
}
