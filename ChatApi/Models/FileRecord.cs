using System;
using System.ComponentModel.DataAnnotations;

namespace ChatApi.Models
{
    public class FileRecord
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FileName { get; set; }
        [Required]
        public bool IsPhoto { get; set; }
        [Required]
        public string Extension { get; set; }
        [Required]
        public string Path { get; set; }
        [Required]
        public DateTime Uploaded { get; set; } = DateTime.Now;
    }
}
