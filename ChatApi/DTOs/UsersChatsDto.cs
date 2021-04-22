using System;

namespace ChatApi.DTOs
{
    public class UsersChatsDto
    {
        public FriendUserReadDto User { get; set; }
        public ChatReadDto Chat { get; set; }
        public DateTime LastSeen { get; set; }
        public bool IsActive { get; set; }
        public int NotSeenMessagesNumber { get; set; }
    }
}
