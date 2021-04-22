using AutoMapper;
using ChatApi.DTOs;
using ChatApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<AppUser, UserReadDto>();
            CreateMap<AppUser, FriendUserReadDto>();
            CreateMap<AppUser, LoggedInUserReadDto>()
                .ForMember(dest => dest.Friends, o => o.MapFrom(source => source.FriendShips.Select(fs => fs.Friend.User)));
        }
    }
}
