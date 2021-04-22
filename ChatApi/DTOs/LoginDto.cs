using System.ComponentModel.DataAnnotations;

namespace ChatApi.DTOs
{
    public class LoginDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        public bool StayLoggedIn { get; set; } = false;
    }
}
