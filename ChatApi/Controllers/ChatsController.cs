using AutoMapper;
using ChatApi.DbAccess;
using ChatApi.Models;
using ChatApi.ServicesInterfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApi.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace ChatApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : Controller
    {
        private readonly IChatsManager _chatsManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChatsController(
            IChatsManager chatsManager,
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _chatsManager = chatsManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateChat([FromBody] ChatCreateDto chatCreateDto)
        {
            var user = _unitOfWork.Users.GetByID(chatCreateDto.OwnerId);
            if (user == null)
            {
                return NotFound();
            }
            var chat = new Chat
            {
                Name = chatCreateDto.Name,
                IsGroupChat = true,
                Owner = user
            };
            var userChat = new UsersChats
            {
                User = user,
                Chat = chat
            };
            chat.UsersChats.Add(userChat);
            await _chatsManager.CreateChatAsync(chat);
            return Created("", chatCreateDto);
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteChat(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var chat = _unitOfWork.Chats.GetByID(id);
            if (chat == null)
            {
                return NotFound();
            }
            if (chat.Owner != user)
            {
                return Unauthorized();
            }
            await _chatsManager.DeleteChatAsync(id);
            return Ok(id);
        }

        [HttpPost]
        [Route("addToChat")]
        public async Task<IActionResult> AddToChat([FromBody] AddOrRemoveUserToChatDto addUserToChatDto)
        {
            var user = await _userManager.GetUserAsync(User);
            var chat = _unitOfWork.Chats.GetByID(addUserToChatDto.ChatId);
            if (chat == null)
            {
                return NotFound();
            }

            if (chat.Owner != user)
            {
                return Unauthorized();
            }
            if (chat.UsersChats.Where(uc => uc.User.Id == addUserToChatDto.UserId).Count() > 0)
            {
                return Conflict(addUserToChatDto);
            }
            await _chatsManager.AddUserToChatAsync(addUserToChatDto.ChatId, addUserToChatDto.UserId);
            return Ok(addUserToChatDto);
        }

        [HttpPost]
        [Route("removeFromChat")]
        public async Task<IActionResult> RemoveFromChat([FromBody] AddOrRemoveUserToChatDto removeUserFromChatDto)
        {
            var user = await _userManager.GetUserAsync(User);
            var userToRemove = _unitOfWork.Users.GetByID(removeUserFromChatDto.UserId);
            if (userToRemove == null)
            {
                return NotFound();
            }
            if (user == userToRemove)
            {
                await _chatsManager.RemoveUserFromChatAsync(removeUserFromChatDto.ChatId, removeUserFromChatDto.UserId);
                return Ok(removeUserFromChatDto);
            }
            var chat = _unitOfWork.Chats.GetByID(removeUserFromChatDto.ChatId);
            if (chat.Owner == user)
            {
                await _chatsManager.RemoveUserFromChatAsync(removeUserFromChatDto.ChatId, removeUserFromChatDto.UserId);
                return Ok(removeUserFromChatDto);
            }
            return Unauthorized();
        }
    }
}
