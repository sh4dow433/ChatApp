using AutoMapper;
using ChatApi.DbAccess;
using ChatApi.DTOs;
using ChatApi.Hubs;
using ChatApi.Models;
using ChatApi.ServicesInterfaces;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Services
{
    public class ChatsManager : IChatsManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConnectionsManager _connectionsManager;
        private readonly IHubContext<ChatHub> _hub;
        private readonly IMapper _mapper;
        private readonly ConcurrentDictionary<AppUser, string> _connectedUsers;
        private readonly JsonSerializerSettings _settings;

        public ChatsManager(IUnitOfWork unitOfWork,
            IConnectionsManager connectionsManager,
            IHubContext<ChatHub> hub,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _connectionsManager = connectionsManager;
            _hub = hub;
            _mapper = mapper;
            _connectedUsers = _connectionsManager.ConnectedUsersByAppUser;

            _settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public async Task SendMessageAsync(Message message)
        {
            if (message == null)
                return;
            message.Text = StringSanitizer.CleanMessage(message.Text);

            _unitOfWork.Messages.Insert(message);
            _unitOfWork.SaveChanges();

            var messageReadDto = _mapper.Map<MessageReadDto>(message);
            var messageReadDtoString = JsonConvert.SerializeObject(messageReadDto, _settings);

            foreach (var userChat in message.Chat.UsersChats.ToList())
            {
                if (_connectedUsers.ContainsKey(userChat.User))
                {
                    await _hub.Clients.Client(_connectedUsers[userChat.User]).SendAsync("ReceiveMessage", messageReadDtoString);

                    if (userChat.IsActive == true)
                    {
                        await ChatSeenAsync(userChat.Chat.Id, userChat.User.Id);
                    }
                    else
                    {
                        userChat.NotSeenMessagesNumber++;
                        _unitOfWork.SaveChanges();
                    }
                }
                else
                {
                    userChat.NotSeenMessagesNumber++;
                    _unitOfWork.SaveChanges();
                }
            }
        }

        public async Task DeleteMessageAsync(int id)
        {
            var message = _unitOfWork.Messages.GetByID(id);
            if (message == null)
                return;
            message.IsRemoved = true;
            foreach (var userChat in message.Chat.UsersChats.ToList())
            {
                var connectedUsers = _connectionsManager.ConnectedUsersByAppUser;
                if (connectedUsers.ContainsKey(userChat.User))
                {
                    await _hub.Clients.Client(connectedUsers[userChat.User]).SendAsync("MessageDeleted", message.Chat.Id, id);
                }
            }
        }

        public async Task ChatSeenAsync(int chatId, string userId)
        {
            var userChat = _unitOfWork.UsersChats.Get(uc => uc.User.Id == userId && uc.Chat.Id == chatId).FirstOrDefault();
            if (userChat == null)
                return;

            // set all chats to inactive
            foreach(var inactiveUserChat in _unitOfWork.UsersChats.Get(uc => uc.User.Id == userId).ToList())
            {
                inactiveUserChat.IsActive = false;
            }

            // set current chat to active
            userChat.IsActive = true;
            userChat.LastSeen = DateTime.Now;
            userChat.NotSeenMessagesNumber = 0;
            _unitOfWork.SaveChanges();

            foreach (var user in userChat.Chat.UsersChats.Select(uc => uc.User).ToList())
            {
                if (_connectedUsers.ContainsKey(user))
                {
                    await _hub.Clients.Client(_connectedUsers[user]).SendAsync("ChatSeen", chatId, userId, userChat.LastSeen);
                }
            }
        }



        public async Task CreateChatAsync(Chat chat)
        {
            ///////////////
            if (chat == null)
                return;
            chat.Name = StringSanitizer.CleanName(chat.Name);

            _unitOfWork.Chats.Insert(chat);
            _unitOfWork.SaveChanges();           

            var chatReadDto = _mapper.Map<ChatReadDto>(chat);
            var chatReadDtoString = JsonConvert.SerializeObject(chatReadDto,_settings);
            foreach (var user in chat.UsersChats.Select(uc => uc.User))
            {
                if (_connectedUsers.ContainsKey(user))
                {
                    await _hub.Clients.Client(_connectedUsers[user]).SendAsync("NewChatCreated", chatReadDtoString);
                }
            }
        }

        public async Task DeleteChatAsync(int chatId)
        {
            var chat = _unitOfWork.Chats.GetByID(chatId);
            if (chat == null)
                return;
            _unitOfWork.Chats.Delete(chat);
            _unitOfWork.SaveChanges();

            foreach (var user in chat.UsersChats.Select(uc => uc.User))
            {
                bool refresh = false;
                if (user.UsersChats.Where(uc => uc.IsActive).Count() == 0)
                {
                    refresh = true;
                    var uc = user.UsersChats.FirstOrDefault();
                    if (uc != null)
                    {
                        uc.IsActive = true;
                    }
                    _unitOfWork.Update(user);
                    _unitOfWork.SaveChanges();
                }
      
                if (_connectedUsers.ContainsKey(user))
                {
                    await _hub.Clients.Client(_connectedUsers[user]).SendAsync("ChatDeleted", chatId, refresh);
                }
            }
        }

        public async Task UpdateChatNameAsync(int chatId, string name)
        {
            var chat = _unitOfWork.Chats.GetByID(chatId);
            //
            if (chat == null)
                return;

            chat.Name = StringSanitizer.CleanName(chat.Name);
            chat.Name = name;

            _unitOfWork.SaveChanges();

            foreach (var user in chat.UsersChats.Select(uc => uc.User))
            {
                var connectedUsers = _connectionsManager.ConnectedUsersByAppUser;
                if (connectedUsers.ContainsKey(user))
                {
                    await _hub.Clients.Client(connectedUsers[user]).SendAsync("ChatNameChanged", chatId, name);
                }
            }
        }

        public async Task AddUserToChatAsync(int chatId, string userId)
        {
            var chat = _unitOfWork.Chats.GetByID(chatId);
            var userToAdd = _unitOfWork.Users.GetByID(userId);
            var userChat = new UsersChats()
            {
                Chat = chat,
                User = userToAdd
            };
            _unitOfWork.UsersChats.Insert(userChat);
            _unitOfWork.SaveChanges();

            var chatReadDto = _mapper.Map<ChatReadDto>(chat);
            var chatReadDtoString = JsonConvert.SerializeObject(chatReadDto, _settings);

            var userChatDto = _mapper.Map<UsersChatsDto>(userChat);
            var userChatDtoString = JsonConvert.SerializeObject(userChatDto, _settings);

            foreach (var user in chat.UsersChats.Select(uc => uc.User))
            {
                if (_connectedUsers.ContainsKey(user))
                {
                    if (user == userToAdd)
                    {
                        await _hub.Clients.Client(_connectedUsers[user]).SendAsync("NewChatCreated", chatReadDtoString);
                    }
                    await _hub.Clients.Client(_connectedUsers[user]).SendAsync("UserAddedToChat", userChatDtoString, chatId);
                }
            }
        }

        public async Task RemoveUserFromChatAsync(int chatId, string userId)
        {
            var chat = _unitOfWork.Chats.GetByID(chatId);
            var userToRemove = _unitOfWork.Users.GetByID(userId);
            var userChat = _unitOfWork.UsersChats.Get(uc => uc.Chat == chat && uc.User == userToRemove);

            _unitOfWork.UsersChats.Delete(userChat);
            _unitOfWork.SaveChanges();

            var chatReadDto = _mapper.Map<ChatReadDto>(chat);
            var chatReadDtoString = JsonConvert.SerializeObject(chatReadDto, _settings);
            
            foreach (var user in chat.UsersChats.Select(uc => uc.User))
            {
                if (_connectedUsers.ContainsKey(user))
                {
                    if (user == userToRemove)
                    {
                        await _hub.Clients.Client(_connectedUsers[user]).SendAsync("ChatDeleted", chatId);
                    }
                    await _hub.Clients.Client(_connectedUsers[user]).SendAsync("UserRemovedFromChat", userId, chatId);
                }
            }
        }
    }
}
