using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.DbAccess
{
    public class DbHelper
    {
        private readonly AppDbContext _dbContext;

        public DbHelper(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void SetAllUsersToInactive()
        {
            var users = _dbContext.Users.ToList();
            users.ForEach(u => u.IsActive = false);
            _dbContext.SaveChanges();
        }
    }
}
