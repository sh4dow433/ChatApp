using System;
using System.Collections.Generic;

namespace ChatApi.DTOs
{
    public class LoggedInUserReadDto
    {
        public string Id { get; set; }
        public int ProfilePicId { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastOnline { get; set; }
        public List<UsersChatsDto> UsersChats { get; set; }
        public List<FriendUserReadDto> Friends { get; set; }
    }
}
