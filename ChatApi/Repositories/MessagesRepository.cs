using ChatApi.DbAccess;
using ChatApi.Models;
using ChatApi.Repositories.Interfaces;


namespace ChatApi.Repositories
{
    public class MessagesRepository : BaseRepository<Message>, IMessagesRepository
    {
        public MessagesRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
