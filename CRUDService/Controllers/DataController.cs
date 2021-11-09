using AutoMapper;
using CRUDService.Helpers;
using CRUDService.Models;
using CRUDService.Services;
using DTOModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRUDService.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class DataController : Controller
    {
        private readonly IDataService _dataservice;
        private readonly IMapper _mapper;
        public DataController(IDataService userservice, IMapper mapper)
        {
            _dataservice = userservice;
            _mapper = mapper;
        }

        [HttpPost("RegisterNewUser")]
        public async Task<IActionResult> RegisterNewUserAsync([FromBody]UsersDTO user)
        {
            try
            {
                var createduser = await _dataservice.RegisterUserAsync(user, user.Password);
                return Json(createduser);
            }
            catch (AppException ex)
            {
                return Json(ex.InnerException);
            }
        }

        [HttpPost("AuthenticateUser")]
        public async Task<IActionResult> AuthenticateUserAsync([FromBody]UsersDTO user)
        {
            try
            {

                var authenticateduser = await _dataservice.AuthenticateUser(user, user.Password);
                return Json(authenticateduser);
            }
            catch (AppException ex)
            {
                return Json(ex.InnerException);
            }
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users =  _dataservice.GetUsers();
                return Json(users);
            }
            catch (AppException ex)
            {
                return Json(ex.InnerException);
            }
        }

        [HttpGet("GetConversations")]
        public async Task<IActionResult> GetConversations(string userid)
        {
            try
            {
                var conversations = await _dataservice.GetConversations(userid);
                return Json(conversations);
            }
            catch (AppException ex)
            {
                return Json(ex.InnerException);
            }
        }

        [HttpGet("GetConversationMessages")]
        public IActionResult GetConversationMessages(string conversationid, string userid)
        {
            try
            {
                var conversationsmessages =  _dataservice.GetConversationsMessages(conversationid, userid);
                return Json(conversationsmessages);
            }
            catch (AppException ex)
            {
                return Json(ex.InnerException);
            }
        }

        [HttpPost("SaveNewConversation")]
        public IActionResult SaveNewConversation(ConversationsDTO conversation)
        {
            try
            {
                var conversationsaved = _dataservice.SaveNewConversation(conversation);
                return Json(conversationsaved);
            }
            catch (AppException ex)
            {
                return Json(ex.InnerException);
            }
        }

        [HttpPost("SaveMessage")]
        public IActionResult SaveMessage(MessagesDTO message)
        {
            try
            {
                var messagesaved = _dataservice.SaveMessage(message);
                return Json(messagesaved);
            }
            catch (AppException ex)
            {
                return Json(ex.InnerException);
            }
        }
    }
}
