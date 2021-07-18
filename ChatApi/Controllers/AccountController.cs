using ChatApi.DTOs;
using ChatApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ChatApi.DbAccess;
using AutoMapper;

namespace ChatApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly JwtTokenConfig _jwtTokenConfig;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountController(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            JwtTokenConfig jwtTokenConfig,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtTokenConfig = jwtTokenConfig;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        [HttpGet]
        [Route("user/full/{id}")]
        public async Task<IActionResult> GetFull(string id)
        {
            var currentUser = await _signInManager.UserManager.GetUserAsync(User);
            var user = _unitOfWork.Users.GetByID(id);
            if (user == null)
            {
                return NotFound();
            }
            if (currentUser == null || user.Id != currentUser.Id)
            {
                return Unauthorized();
            }
            var result = _mapper.Map<LoggedInUserReadDto>(user);
            return Ok(result);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (ModelState.IsValid == false)
            {
                return ValidationProblem(ModelState);
            }

            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null)
            {
                return Unauthorized();
            }

            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (passwordCheck.Succeeded == false)
            {
                return Unauthorized();
            }
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            DateTime expirationDate = DateTime.Now.AddHours(3);
            if (loginDto.StayLoggedIn)
            {
                expirationDate = DateTime.Now.AddDays(3);
                claims.Add(new Claim("isPersistent", "true"));
            }
            else
            {
                claims.Add(new Claim("isPersistent", "false"));
            }

            var secretBytes = Encoding.UTF8.GetBytes(_jwtTokenConfig.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            var algorithm = SecurityAlgorithms.HmacSha256;

            var signingCredentials = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(
                _jwtTokenConfig.Issuer,
                _jwtTokenConfig.Audience,
                claims,
                notBefore: DateTime.Now,
                expires: expirationDate,
                signingCredentials);

            var tokenJson = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { AccessToken = tokenJson });
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (ModelState.IsValid == false)
            {
                return ValidationProblem(ModelState);
            }
           
            var existingUser = await _userManager.FindByNameAsync(registerDto.UserName);
            var existingEmail = await _userManager.FindByEmailAsync(registerDto.Email);
            if ( existingUser != null || existingEmail != null)
            {
                return Conflict();
            }

            AppUser user = new AppUser()
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                ////////////adauga rol aici
                var friend = new Friend()
                {
                    User = user
                };
                _unitOfWork.Friends.Insert(friend);
                _unitOfWork.SaveChanges();
                return Created("", registerDto);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("newEmail")]
        public async Task<IActionResult> ChangeEmail([FromBody] NewEmailDto newEmailDto)
        {
            var newEmail = newEmailDto.NewEmail;
            var id = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id.Value);
            if (newEmail == user.Email)
            {
                return ValidationProblem("New email can't be the same as the old one");
            }
           
            if (await _userManager.FindByEmailAsync(newEmail) != null)
            {
                return Conflict("Email is already in use");
            }
            user.Email = newEmail;
            var result = await _userManager.UpdateAsync(user);
            if(result.Succeeded)
            {
                return Ok();
            }
            return Problem();
        }

        [HttpPost]
        [Route("newPassword")]
        public async Task<IActionResult> ChangePassowrd([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (ModelState.IsValid == false)
            {
                return ValidationProblem(ModelState);
            }
            var id = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(id.Value);

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.Password);
            if (result.Succeeded)
            {
                return Ok();
            }
            return Problem();
        }
        [HttpPost]
        [Route("newUsername")]
        public async Task<IActionResult> ChangeName([FromBody] string newUsername)
        {
            var id = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(id.Value);
            if (await _userManager.FindByNameAsync(newUsername) != null)
            {
                return Conflict();
            }
            user.UserName = newUsername;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok();
            }
            return Problem();
        }
    }
}
