using ChatService.Models;
using DTOModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;
        private IConfiguration _Configuration { get; }
        public ChatHub(IHttpContextAccessor httpContextAccessor,IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
            _Configuration = configuration;
        }

        //public static ConcurrentDictionary<string, string> Connections = new ConcurrentDictionary<string, string>();
        public async Task SendMessage(string user, MessagesDTO message)
        {
            try {
                message.TimeStamp = DateTime.Now;
                Guid userid = Guid.Parse(Context.UserIdentifier);
                var savemessagetask = this.SaveMessage(message, userid);
                //Connections.TryGetValue(user, out connectionToSendMessage);

                if (!string.IsNullOrWhiteSpace(user))
                {
                    var signalrmessage = new SignalRMessage() { Type = "msg", Message = JsonConvert.SerializeObject(message) };
                    await Clients.Users(user).SendAsync("broadcastMessage", JsonConvert.SerializeObject(signalrmessage));
                }
                await savemessagetask;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            }

        public void BroadcastMessage(string message)
        {
            Clients.All.SendAsync("broadcastMessage", Context.User.Identity.Name, message);
        }



        public override Task OnConnectedAsync()
        {
            //if (!Connections.ContainsKey(Context.ConnectionId))
            //{
            //    Connections.TryAdd(Context.UserIdentifier, Context.ConnectionId);
            //}
            var message = new SignalRMessage() { Type = "joined", Message = Context.UserIdentifier };
            Clients.All.SendAsync("broadcastMessage", JsonConvert.SerializeObject(message));
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            //if (!Connections.ContainsKey(Context.ConnectionId))
            //{
            //    Connections.TryRemove(Context.UserIdentifier, out var value);
            //}
            var message = new SignalRMessage() { Type = "left", Message = Context.UserIdentifier };
            Clients.All.SendAsync("broadcastMessage", JsonConvert.SerializeObject(message));
            return base.OnConnectedAsync();
        }

        //public void GetAllUsers()
        //{
        //    var users = JsonConvert.SerializeObject(Connections.Select(t => t.Key));
        //    var message = new SignalRMessage() { Type = "Allusers", Message = users };
        //    Clients.All.SendAsync("broadcastMessage", JsonConvert.SerializeObject(message));
        //}


        private async Task<bool> SaveMessage(MessagesDTO message, Guid userid)
        {
            try
            {
                message.UserId = userid;
                var CrudUrl = _Configuration.GetSection("AppSettings:CrudUrl").Value.ToString();
                var request = new HttpRequestMessage(HttpMethod.Post, CrudUrl + "/data/SaveMessage");
                request.Content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
                var client = _clientFactory.CreateClient();
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var savedconversation = JsonConvert.DeserializeObject<ConversationsDTO>(await response.Content.ReadAsStringAsync());
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
