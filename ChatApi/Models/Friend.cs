using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Models
{
    public class Friend
    {
        [Key]
        public int Id { get; set; }
        public List<FriendShip> FriendShips { get; set; } = new List<FriendShip>();

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }
    }
}
