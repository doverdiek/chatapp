using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatService.Models
{
    public class SignalRUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
        }
    }
}
