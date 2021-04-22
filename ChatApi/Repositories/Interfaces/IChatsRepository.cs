using ChatApi.Models;
using System.Collections.Generic;

namespace ChatApi.Repositories.Interfaces
{
    public interface IChatsRepository : IRepository<Chat>
    {
        IEnumerable<Chat> GetAllChatsFromUser(AppUser user);
        IEnumerable<Chat> GetAllChatsFromUser(int userId);

    }
}
