using ChatApi.DbAccess;
using ChatApi.Hubs;
using ChatApi.Models;
using ChatApi.ServicesInterfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Services
{
    public class ConnectionsManager : IConnectionsManager
    {
        private readonly IHubContext<ChatHub> _hub;
        private readonly IUnitOfWork _unitOfWork;

        private static ConcurrentDictionary<string, AppUser> _connUsersByConId = new ConcurrentDictionary<string, AppUser>();
        private static ConcurrentDictionary<AppUser, string> _connUsersByAppUser = new ConcurrentDictionary<AppUser, string>();
      
        public ConcurrentDictionary<string, AppUser> ConnectedUsersByConId { get => _connUsersByConId;} 
        public ConcurrentDictionary<AppUser, string> ConnectedUsersByAppUser { get => _connUsersByAppUser; }

        public ConnectionsManager(IHubContext<ChatHub> hub,
                IUnitOfWork unitOfWork)
        {
            _hub = hub;
            _unitOfWork = unitOfWork;
        }

        public async Task ConnectUserAsync(string connId, AppUser user)
        {
            ConnectedUsersByConId[connId] = user;
            ConnectedUsersByAppUser[user] = connId;

            user = _unitOfWork.Users.GetByID(user.Id);
            user.IsActive = true;
            _unitOfWork.SaveChanges();

            foreach (var friend in user.FriendShips.Select(fs => fs.Friend).Select(f => f.User).Where(u => u.IsActive))
            {
                if (ConnectedUsersByAppUser.ContainsKey(friend))
                {
                    var connectionId = ConnectedUsersByAppUser[friend];
                    await _hub.Clients.Client(connectionId).SendAsync("FriendOnline", user.Id);
                }
            }
        }

        public async Task DisconnectUserAsync(string connId)
        {
            if (ConnectedUsersByConId.ContainsKey(connId) == false)
            {
                return;
            }
            ConnectedUsersByConId.TryRemove(connId, out AppUser user);
            if (ConnectedUsersByAppUser.ContainsKey(user))
            {
                ConnectedUsersByAppUser.TryRemove(user, out _);
            }

            user = _unitOfWork.Users.GetByID(user.Id);
            user.IsActive = false;
            _unitOfWork.Update(user);
            _unitOfWork.SaveChanges();

            foreach (var friend in user.FriendShips.Select(fs => fs.Friend).Select(f => f.User).Where(u => u.IsActive))
            {
                if (ConnectedUsersByAppUser.ContainsKey(friend))
                {
                    var connectionId = ConnectedUsersByAppUser[friend];
                    await _hub.Clients.Client(connectionId).SendAsync("FriendOffline", user.Id);
                }
            }
        }
    }
}
