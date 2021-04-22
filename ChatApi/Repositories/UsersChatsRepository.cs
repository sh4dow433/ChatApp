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
    public class UsersChatsRepository : BaseRepository<UsersChats>, IUsersChatsRepository
    {
        public UsersChatsRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
        public override IEnumerable<UsersChats> Get(Expression<Func<UsersChats, bool>> filter)
        {
            return _dbContext.UsersChats
                    .Include(uc => uc.User)
                    .Include(uc => uc.Chat)
                        .ThenInclude(c => c.UsersChats)
                            .ThenInclude(uc => uc.Chat)
                    .Include(uc => uc.Chat)
                        .ThenInclude(c => c.UsersChats)
                            .ThenInclude(uc => uc.User)
                    .Where(filter)
                    .ToList();
        }
    }
}
