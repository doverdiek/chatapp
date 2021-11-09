using DTOModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private IConfiguration _Configuration { get; }
        public UserController(IHttpClientFactory clientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _Configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("GetAllUsers")]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var userid = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var CrudUrl = _Configuration.GetSection("AppSettings:CrudUrl").Value.ToString();
                var request = new HttpRequestMessage(HttpMethod.Get, CrudUrl + "/data/GetUsers");
                var client = _clientFactory.CreateClient();
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var users = JsonConvert.DeserializeObject<IEnumerable<UsersDTO>>(await response.Content.ReadAsStringAsync());
                    users = users.Where(u => u.UserId != Guid.Parse(userid));
                    return Ok(users);
                }
                return BadRequest("error");
            }
            catch (Exception ex)
            {
                return BadRequest("error");
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UsersDTO user)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(user.FirstName)
                    && !String.IsNullOrWhiteSpace(user.LastName)
                    && !String.IsNullOrWhiteSpace(user.Username)
                    && !String.IsNullOrWhiteSpace(user.Password)
                    )
                {
                    var CrudUrl = _Configuration.GetSection("AppSettings:CrudUrl").Value.ToString();
                    var request = new HttpRequestMessage(HttpMethod.Post, CrudUrl + "/data/RegisterNewUser");
                    request.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    var client = _clientFactory.CreateClient();
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        return Ok(true);
                    }
                    else
                    {
                        Debug.Write(response.ReasonPhrase);
                        return BadRequest(response);
                    }
                }
                return BadRequest("geen correcte user opgegeven");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
