using AutoMapper;
using ChatApi.DbAccess;
using ChatApi.DTOs;
using ChatApi.Models;
using ChatApi.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FriendsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IFriendsManager _friendsManager;
        private readonly IChatsManager _chatsManager;

        public FriendsController(IUnitOfWork unitOfWork,
                SignInManager<AppUser> signInManager,
                IFriendsManager friendsManager,
                IChatsManager chatsManager)
        {
            _unitOfWork = unitOfWork;
            _signInManager = signInManager;
            _friendsManager = friendsManager;
            _chatsManager = chatsManager;
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddFriend([FromBody] AddFriendDto addFriendDto)
        {
            var currentUser = await _signInManager.UserManager.GetUserAsync(User); 
            var user = _unitOfWork.Users.GetByID(addFriendDto.UserId);
            if (user == null)
            {
                return BadRequest();
            }
            if (currentUser.Id != user.Id)
            {
                return Unauthorized();
            }
            var friend = _unitOfWork.Users.GetByName(addFriendDto.FriendsName);

            if (friend == null)
            {
                return NotFound();
            }
            if (user == friend)
            {
                return BadRequest();
            }
            bool result = await _friendsManager.AddFriendAsync(user, friend);

            if (result)
            {

                var chat = new Chat()
                {
                    IsGroupChat = false
                };
                var userChats1 = new UsersChats()
                {
                    User = user,
                    Chat = chat
                };
                var userChats2 = new UsersChats()
                {
                    User = friend,
                    Chat = chat
                };
                chat.UsersChats.Add(userChats1);
                chat.UsersChats.Add(userChats2);
                await _chatsManager.CreateChatAsync(chat);

                return Created("", addFriendDto);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("remove")]
        public async Task<IActionResult> RemoveFriend([FromBody] RemoveFriendDto removeFriendDto)
        {
            var user = await _signInManager.UserManager.FindByIdAsync(removeFriendDto.UserId);
            if (user != await _signInManager.UserManager.GetUserAsync(User))
            {
                return Unauthorized();
            }
            var friend = await _signInManager.UserManager.FindByIdAsync(removeFriendDto.FriendId);
            var result = await _friendsManager.RemoveFriendAsync(user, friend);
            if (result)
            {
                return Ok(removeFriendDto);
            }
            else
            {
                return NotFound();
            }   
        }
    }
}
