using System;
using System.Collections.Generic;

namespace CRUDService.Models
{
    public partial class Users
    {
        public Users()
        {
            Messages = new HashSet<Messages>();
            UsersConversations = new HashSet<UsersConversations>();
        }

        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Picture { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public virtual ICollection<Messages> Messages { get; set; }
        public virtual ICollection<UsersConversations> UsersConversations { get; set; }
    }
}
