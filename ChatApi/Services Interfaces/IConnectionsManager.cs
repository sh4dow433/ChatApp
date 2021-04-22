using ChatApi.Models;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ChatApi.ServicesInterfaces
{
    public interface IConnectionsManager
    {
        ConcurrentDictionary<string, AppUser> ConnectedUsersByConId { get; }
        ConcurrentDictionary<AppUser, string> ConnectedUsersByAppUser { get; }

        Task ConnectUserAsync(string connId, AppUser user);
        Task DisconnectUserAsync(string connId);
    }
}
