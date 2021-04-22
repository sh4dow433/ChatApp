using System;

namespace ChatApi.DTOs
{
    public class MessageReadDto
    {
        public int Id { get; set; }
        public UserReadDto Sender { get; set; }
        public ChatReadDto Chat { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public bool IsRemoved { get; set; }
        public bool HasFileAttached { get; set; }
        public FileReadDto File { get; set; }
    }
}
