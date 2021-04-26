using System;
using System.ComponentModel.DataAnnotations;

namespace ChatApi.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public AppUser Sender { get; set; }
        [Required]
        public Chat Chat { get; set; }
        [Required]
        [MaxLength(250)]
        public string Text { get; set; }
        [Required]
        public DateTime Date { get; set; } = DateTime.Now;
        [Required]
        public bool IsRemoved { get; set; } = false;
        public FileRecord File { get; set; }
    }
}
