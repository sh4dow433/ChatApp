using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Models
{
    public class FriendShip
    {
        public string UserId { get; set; }
        public AppUser User { get; set; }

        public int FriendId { get; set; }
        public Friend Friend { get; set; }

    }
}
