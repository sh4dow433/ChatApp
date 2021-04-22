using System;

namespace ChatApi.DTOs
{
    public class FriendUserReadDto
    {
        public string Id { get; set; }
        public int ProfilePicId { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastOnline { get; set; }
    }
}
