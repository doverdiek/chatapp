using DTOModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace ChatService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ChatMessageController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private IConfiguration _Configuration { get; }

        public ChatMessageController(IHttpClientFactory clientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _Configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("GetConversations")]
        public async Task<IActionResult> GetConversations()
        {
            try
            {
                var userid = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var CrudUrl = _Configuration.GetSection("AppSettings:CrudUrl").Value.ToString();
                var request = new HttpRequestMessage(HttpMethod.Get, CrudUrl + "/data/GetConversations?userid=" + userid);
                var client = _clientFactory.CreateClient();
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var conversations = JsonConvert.DeserializeObject<IEnumerable<ConversationsDTO>>(await response.Content.ReadAsStringAsync());
                    return Ok(conversations);
                }
                return BadRequest("error");
            }
            catch (Exception ex)
            {
                return BadRequest("error");
            }
        }

        [HttpGet("GetConversationMessages")]
        public async Task<IActionResult> GetConversationMessages(Guid? conversationid)
        {
            try
            {
                if (conversationid == null)
                {
                    return Ok();
                }
                var userid = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var CrudUrl = _Configuration.GetSection("AppSettings:CrudUrl").Value.ToString();
                var request = new HttpRequestMessage(HttpMethod.Get, CrudUrl + "/data/GetConversationMessages?conversationid=" + conversationid + "&userid="+ userid);
                var client = _clientFactory.CreateClient();
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var messages = JsonConvert.DeserializeObject<IEnumerable<MessagesDTO>>(await response.Content.ReadAsStringAsync());
                    return Ok(messages);
                }
                return BadRequest("error");
            }
            catch (Exception ex)
            {
                return BadRequest("error");
            }
        }

        [HttpPost("SaveNewConversation")]
        public async Task<IActionResult> SaveNewConversation(ConversationsDTO conversation)
        {
            try
            {
                var userid = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                conversation.UsersConversations.Add(new UsersConversationsDTO { UserId = Guid.Parse(userid) });
                var CrudUrl = _Configuration.GetSection("AppSettings:CrudUrl").Value.ToString();
                var request = new HttpRequestMessage(HttpMethod.Post, CrudUrl + "/data/SaveNewConversation");
                request.Content = new StringContent(JsonConvert.SerializeObject(conversation), Encoding.UTF8, "application/json");
                var client = _clientFactory.CreateClient();
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var savedconversation = JsonConvert.DeserializeObject<ConversationsDTO>(await response.Content.ReadAsStringAsync());
                    return Ok(savedconversation);
                }
                return BadRequest("error");
            }
            catch (Exception ex)
            {
                return BadRequest("error");
            }
        }
    }
}
