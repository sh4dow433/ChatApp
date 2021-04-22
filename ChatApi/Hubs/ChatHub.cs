using AutoMapper;
using ChatApi.DbAccess;
using ChatApi.DTOs;
using ChatApi.Models;
using ChatApi.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace ChatApi.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatsManager _chatsManager;
        private readonly IConnectionsManager _connectionManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ChatHub(IChatsManager chatsManager,
                IConnectionsManager connectionsManager,
                SignInManager<AppUser> signInManager,
                IMapper mapper,
                IUnitOfWork unitOfWork)
        {
            _chatsManager = chatsManager;
            _connectionManager = connectionsManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public override async Task OnConnectedAsync()
        {
            var user = await _signInManager.UserManager.GetUserAsync(Context.User);
            await _connectionManager.ConnectUserAsync(Context.ConnectionId, user);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _connectionManager.DisconnectUserAsync(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string messageCreateDtoString)
        {
            var messageCreateDto = JsonConvert.DeserializeObject<MessageCreateDto>(messageCreateDtoString);
            var message = _mapper.Map<Message>(messageCreateDto);
            message.Chat = _unitOfWork.Chats.GetByID(messageCreateDto.ChatId);
            message.Sender = _unitOfWork.Users.GetByID(messageCreateDto.SenderId);
            await _chatsManager.SendMessageAsync(message);
        }

        public async Task DeleteMessage(int id)
        {
            await _chatsManager.DeleteMessageAsync(id);
        }

        public async Task ChatSeen(string chatSeenDtoString)
        {
            var chatSeenDto = JsonConvert.DeserializeObject<ChatSeenDto>(chatSeenDtoString);
            await _chatsManager.ChatSeenAsync(chatSeenDto.ChatId, chatSeenDto.UserId);
        }
    }
}
