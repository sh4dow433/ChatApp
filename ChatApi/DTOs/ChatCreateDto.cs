using ChatApi.Models;

namespace ChatApi.DTOs
{
    public class ChatCreateDto
    {
        public string Name { get; set; }
        public AppUser Owner { get; set; }
    }
}
