using ChatApi.DbAccess;
using ChatApi.Models;
using ChatApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ChatApi.Repositories
{
    public class MessagesRepository : BaseRepository<Message>, IMessagesRepository
    {
        public MessagesRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
        public override Message GetByID(object id)
        {
            return _dbContext.Messages
                .Include(m => m.Chat)
                    .ThenInclude(c => c.UsersChats)
                        .ThenInclude(uc => uc.User)
                .Include(m => m.Sender)
                .FirstOrDefault(m => m.Id == (int)id);
        }
    }
}
