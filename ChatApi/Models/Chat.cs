using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChatApi.Models
{
    public class Chat
    {
        public int Id { get; set; }
        [MaxLength(20)]
        public string Name { get; set; }
        [Required]
        public bool IsGroupChat { get; set; }
        public AppUser Owner { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>();
        public List<UsersChats> UsersChats { get; set; } = new List<UsersChats>();
    }
}
