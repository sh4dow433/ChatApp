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
    public class ChatController : Controller
    {
        private readonly IChatsManager _chatsManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChatController(
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
            await _chatsManager.DeleteMessageAsync(id);
            return NoContent();
        }
    }
}
