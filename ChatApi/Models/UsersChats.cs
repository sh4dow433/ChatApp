using System;

namespace ChatApi.Models
{
    public class UsersChats
    {
        public string UserId { get; set; }
        public int ChatId { get; set; }

        public AppUser User { get; set; }
        public Chat Chat { get; set; }

        public bool IsActive { get; set; }
        public DateTime LastSeen { get; set; } = DateTime.Now;
        public int NotSeenMessagesNumber { get; set; } = 0;
    }
}
