using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.DTOs
{
    public class AddOrRemoveUserToChatDto
    {
        public int ChatId { get; set; }
        public string UserId { get; set; }
    }
}
