using ChatApi.Models;
using System.Threading.Tasks;

namespace ChatApi.ServicesInterfaces
{
    public interface IFriendsManager
    {
        Task<bool> AddFriendAsync(AppUser user, AppUser friend);
        Task<bool> RemoveFriendAsync(AppUser user, AppUser friend);

    }
}
