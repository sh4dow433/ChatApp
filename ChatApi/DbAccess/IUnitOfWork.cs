using ChatApi.Repositories.Interfaces;
using System;

namespace ChatApi.DbAccess
{
    public interface IUnitOfWork : IDisposable
    {
        IUsersRepository Users { get; }
        IChatsRepository Chats { get; }
        IMessagesRepository Messages { get; }
        IFilesRepository Files { get; }
        IUsersChatsRepository UsersChats { get; }
        IFriendShipsRepository FriendShips { get; }
        IFriendsRepository Friends { get; }
        int SaveChanges();
        void Update(object entity);
    }
}
