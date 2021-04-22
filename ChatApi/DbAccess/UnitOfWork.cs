using ChatApi.Repositories;
using ChatApi.Repositories.Interfaces;
using System;

namespace ChatApi.DbAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;

        public IUsersRepository Users { get; private set; }
        public IChatsRepository Chats { get; private set; }
        public IMessagesRepository Messages { get; private set; }
        public IFilesRepository Files { get; private set; }
        public IUsersChatsRepository UsersChats { get; private set; }
        public IFriendShipsRepository FriendShips { get; private set; }
        public IFriendsRepository Friends { get; private set; }
        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;

            Users = new UsersRepository(dbContext);
            Chats = new ChatsRepository(dbContext);
            UsersChats = new UsersChatsRepository(dbContext);
            Messages = new MessagesRepository(dbContext);
            Files = new FilesRepository(dbContext);
            FriendShips = new FriendShipsRepository(dbContext);
            Friends = new FriendsRepository(dbContext);
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
        public void Update(object entity)
        {
            _dbContext.Update(entity);
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
