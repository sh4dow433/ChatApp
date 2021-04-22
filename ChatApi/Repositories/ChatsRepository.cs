using ChatApi.DbAccess;
using ChatApi.Models;
using ChatApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ChatApi.Repositories
{
    public class ChatsRepository : BaseRepository<Chat>, IChatsRepository
    {
        public ChatsRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
        public override Chat GetByID(object id)
        {
            return _dbContext.Chats
               .Include(c => c.Messages)
                        //.OrderByDescending(m => m.Date)
                        //.Take(25))
                    .ThenInclude(m => m.Sender)
                .Include(c => c.UsersChats)
                    .ThenInclude(uc => uc.User)
                        .ThenInclude(u => u.FriendShips)
                            .ThenInclude(fs => fs.Friend)
                                .ThenInclude(f => f.User)
                .FirstOrDefault(c => c.Id == (int)id);
        }
        public override IEnumerable<Chat> GetAll()
        {
            return _dbContext.Chats
                .Include(c => c.Messages
                        .OrderByDescending(m => m.Date)
                        .Take(1))
                    .ThenInclude(m => m.Sender)
                        .ThenInclude(u => u.FriendShips)
                            .ThenInclude(fs => fs.Friend)
                                .ThenInclude(f => f.User)
                .Include(c => c.UsersChats)
                .ToList();
        }
        public override IEnumerable<Chat> Get(Expression<Func<Chat, bool>> filter)
        {
            return _dbContext.Chats
                .Include(c => c.Messages
                        .OrderByDescending(m => m.Date))
                    .ThenInclude(m => m.Sender)
                        .ThenInclude(u => u.FriendShips)
                            .ThenInclude(fs => fs.Friend)
                                .ThenInclude(f => f.User)
                .Include(c => c.UsersChats)
                .Where(filter)
                .ToList();
        }

        public IEnumerable<Chat> GetAllChatsFromUser(AppUser user)
        {
            return _dbContext.Chats
               .Include(c => c.Messages
                        .OrderByDescending(m => m.Date)
                        .Take(1))
                   .ThenInclude(m => m.Sender)
               .Include(c => c.UsersChats.Where(uc => uc.User == user))
                   .ThenInclude(uc => uc.User)
               .ToList();
        }

        public IEnumerable<Chat> GetAllChatsFromUser(int userId)
        {
            var user = _dbContext.Users.Find(userId);
            return GetAllChatsFromUser(user);
        }
    }
}
