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
    public class UsersRepository : BaseRepository<AppUser>, IUsersRepository
    {
        public UsersRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
        public override AppUser GetByID(object id)
        {
            return _dbContext.Users
                .Include(u => u.Friend)
                .Include(u => u.FriendShips)
                    .ThenInclude(fs => fs.Friend)
                        .ThenInclude(f => f.User)
                .Include(u => u.UsersChats)
                    .ThenInclude(uc => uc.Chat)
                        .ThenInclude(c => c.UsersChats)
                            .ThenInclude(uc => uc.User)
                .Include(u => u.UsersChats)
                    .ThenInclude(uc => uc.Chat)
                        .ThenInclude(c => c.Messages)
                            .ThenInclude(m => m.Sender)
                .Include(u => u.UsersChats)
                    .ThenInclude(uc => uc.Chat)
                        .ThenInclude(c => c.Messages)
                            .ThenInclude(m => m.Chat)
                                .ThenInclude(c => c.UsersChats)
                                    .ThenInclude(uc => uc.User)
                .FirstOrDefault(u => u.Id == (string)id);
        }
        public override IEnumerable<AppUser> Get(Expression<Func<AppUser, bool>> filter)
        {
            return _dbContext.Users
            .Include(u => u.Friend)
            .Include(u => u.FriendShips)
                .ThenInclude(fs => fs.Friend)
                    .ThenInclude(f => f.User)
            .Where(filter)
            .ToList();
        }

        public AppUser GetByName(string name)
        {
            return _dbContext.Users
            .Include(u => u.Friend)
            .Include(u => u.FriendShips)
                .ThenInclude(fs => fs.Friend)
                    .ThenInclude(f => f.User)
            .FirstOrDefault(u => u.UserName == name);
        }
    }
}
