using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.DTOs
{
    public class AddFriendDto
    {
        public string UserId { get; set; }
        public string FriendsName { get; set; }
    }
}
