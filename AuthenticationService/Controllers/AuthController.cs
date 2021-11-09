using AuthenticationService.Models.Requests;
using DTOModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        

        private  IConfiguration _Configuration { get; }

        public AuthController(IHttpClientFactory clientFactory,
            IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _Configuration = configuration;
           
        }




        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UsersDTO loginRequest)
        {
            try
            {
                
                var CrudUrl = _Configuration.GetSection("AppSettings:CrudUrl").Value.ToString();
                var request = new HttpRequestMessage(HttpMethod.Post, CrudUrl + "/data/AuthenticateUser");
                request.Content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");
                var client = _clientFactory.CreateClient();
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var user = JsonConvert.DeserializeObject<UsersDTO>(await response.Content.ReadAsStringAsync());

                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString(), ClaimValueTypes.String));
                    claims.Add(new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName, ClaimValueTypes.String));
                    string securityKey = "testtesttesttesttesttesttesttesttesttesttesttest";
                    var symetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

                    var signingCredentials = new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

                    var token = new JwtSecurityToken(
                            issuer: "test.nl",
                            audience: "users",
                            expires: DateTime.Now.AddHours(8),
                            signingCredentials: signingCredentials,
                            claims: claims
                    );
                    user.Token = new JwtSecurityTokenHandler().WriteToken(token);
                    return Ok(user);
                }
                else
                {
                    Debug.Write(response.ReasonPhrase);
                    return BadRequest("error");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException);
            }
        }




    }
}
