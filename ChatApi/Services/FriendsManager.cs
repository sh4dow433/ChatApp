using AutoMapper;
using ChatApi.DbAccess;
using ChatApi.DTOs;
using ChatApi.Hubs;
using ChatApi.Models;
using ChatApi.ServicesInterfaces;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Services
{
    public class FriendsManager : IFriendsManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConnectionsManager _connectionsManager;
        private readonly IHubContext<ChatHub> _hub;
        private readonly IMapper _mapper;
        private readonly ConcurrentDictionary<AppUser, string> _connectedUsers;

        public FriendsManager(IUnitOfWork unitOfWork,
            IConnectionsManager connectionsManager,
            IHubContext<ChatHub> hub,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _connectionsManager = connectionsManager;
            _hub = hub;
            _mapper = mapper;
            _connectedUsers = _connectionsManager.ConnectedUsersByAppUser;
        }

        public async Task<bool> AddFriendAsync(AppUser user, AppUser friendUser)
        {
            if (user.FriendShips.Where(fs => fs.Friend.User == friendUser).Count() > 0)
            {
                return false;
            }
            var friend = friendUser.Friend;
            var friendShip = new FriendShip()
            {
                User = user,
                Friend = friend
            };
            var friendShip1 = new FriendShip()
            {
                User = friendUser,
                Friend = user.Friend
            };

            _unitOfWork.FriendShips.Insert(friendShip);
            _unitOfWork.FriendShips.Insert(friendShip1);
            _unitOfWork.SaveChanges();

            if (_connectedUsers.ContainsKey(user))
            {
                var friendDto = _mapper.Map<FriendUserReadDto>(friendUser);
                var friendDtoString = JsonConvert.SerializeObject(friendDto);
                await _hub.Clients.Client(_connectedUsers[user]).SendAsync("NewFriend", friendDtoString);
            }
            if (_connectedUsers.ContainsKey(friendUser))
            {
                var userDto = _mapper.Map<FriendUserReadDto>(user);
                var userDtoString = JsonConvert.SerializeObject(userDto);
                await _hub.Clients.Client(_connectedUsers[friendUser]).SendAsync("NewFriend", userDtoString);
            }
            return true;
        }

        public async Task<bool> RemoveFriendAsync(AppUser user, AppUser friendUser)
        {
            if (user.FriendShips.Where(fs => fs.Friend.User == friendUser).Count() != 1)
            {
                return false;
            }
            var chatToDelete = user.UsersChats
                .Where(uc => uc.Chat.UsersChats.FirstOrDefault(ucc => ucc.User == friendUser) != null)
                .Select(uc => uc.Chat)
                .FirstOrDefault();

            var chatToDeleteId = chatToDelete.Id;

            var friend = friendUser.Friend;
            var friendShip = _unitOfWork.FriendShips.Get(fs => fs.Friend == friend && fs.User == user).FirstOrDefault();
            var friendShip1 = _unitOfWork.FriendShips.Get(fs => fs.Friend == user.Friend && fs.User == friendUser).FirstOrDefault();

            _unitOfWork.Chats.Delete(chatToDelete);
            _unitOfWork.FriendShips.Delete(friendShip);
            _unitOfWork.FriendShips.Delete(friendShip1);
            _unitOfWork.SaveChanges();

            var refreshForUser = false;
            var refreshForFriend = false;

            if (user.UsersChats.Where(uc => uc.IsActive).Count() == 0)
            {
                refreshForUser = true;
                var uc = user.UsersChats.FirstOrDefault();
                if (uc != null)
                {
                    uc.IsActive = true;
                }
                _unitOfWork.Update(user);
                _unitOfWork.SaveChanges();
            }

            if (friendUser.UsersChats.Where(uc => uc.IsActive).Count() == 0)
            {
                refreshForFriend = true;
                var uc = user.UsersChats.FirstOrDefault();
                if (uc != null)
                {
                    uc.IsActive = true;
                }
                _unitOfWork.Update(friendUser);
                _unitOfWork.SaveChanges();
            }

            if (_connectedUsers.ContainsKey(user))
            {
                var friendDto = _mapper.Map<FriendUserReadDto>(friendUser);
                var friendDtoString = JsonConvert.SerializeObject(friendDto);
                await _hub.Clients.Client(_connectedUsers[user]).SendAsync("ChatDeleted", chatToDeleteId, refreshForUser);
            }
            if (_connectedUsers.ContainsKey(friendUser))
            {
                var userDto = _mapper.Map<FriendUserReadDto>(user);
                var userDtoString = JsonConvert.SerializeObject(userDto);
                await _hub.Clients.Client(_connectedUsers[friendUser]).SendAsync("ChatDeleted", chatToDeleteId, refreshForFriend);
            }
            return true;
        }
    }
}
