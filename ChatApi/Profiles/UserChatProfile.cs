using AutoMapper;
using ChatApi.DTOs;
using ChatApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Profiles
{
    public class UserChatProfile : Profile
    {
        public UserChatProfile()
        {
            CreateMap<UsersChats, UsersChatsDto>();
            CreateMap<UsersChatsDto, UsersChats>();
        }
    }
}
