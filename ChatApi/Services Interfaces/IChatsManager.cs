using ChatApi.Models;
using System.Threading.Tasks;

namespace ChatApi.ServicesInterfaces
{
    public interface IChatsManager
    {
        Task SendMessageAsync(Message message);
        Task DeleteMessageAsync(int id);

        Task CreateChatAsync(Chat chat);
        Task DeleteChatAsync(int chatId);

        Task ChatSeenAsync(int chatId, string userId);
        Task UpdateChatNameAsync(int chatId, string name);

        Task AddUserToChatAsync(int chatId, string userId);
        Task RemoveUserFromChatAsync(int chatId, string userId);


    }
}
