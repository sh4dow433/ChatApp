using System.Collections.Generic;

namespace ChatApi.DTOs
{
    public class ChatReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsGroupChat { get; set; }
        public FriendUserReadDto Owner { get; set; }
        public List<MessageReadDto> Messages { get; set; }
        public List<UsersChatsDto> UsersChats { get; set; }
    }
}
