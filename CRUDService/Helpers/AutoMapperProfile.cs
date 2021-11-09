using AutoMapper;
using CRUDService.Models;
using DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRUDService.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Users, UsersDTO>();
            CreateMap<UsersDTO, Users>();

            CreateMap<Conversations, ConversationsDTO>();
            CreateMap<ConversationsDTO, Conversations>();

            CreateMap<Messages, MessagesDTO>();
            CreateMap<MessagesDTO, Messages>();

            CreateMap<UsersConversations, UsersConversationsDTO>();
            CreateMap<UsersConversationsDTO, UsersConversations>();
        }
    }
}
