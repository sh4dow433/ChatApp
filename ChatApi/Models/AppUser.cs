using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ChatApi.Models
{
    public class AppUser : IdentityUser
    {
        public FileRecord ProfilePic { get; set; }
        public Friend Friend { get; set; }

        public List<FriendShip> FriendShips { get; set; } = new List<FriendShip>();
        public List<UsersChats> UsersChats { get; set; } = new List<UsersChats>();

        public bool IsActive { get; set; }
        public DateTime LastOnline { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            AppUser other = obj as AppUser;
            return other != null && other.Id == Id;
        }
    }
}
