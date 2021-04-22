using AutoMapper;
using ChatApi.DTOs;
using ChatApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Profiles
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
            CreateMap<ChatCreateDto, Chat>();
            CreateMap<Chat, ChatReadDto>();
        }
    }
}
