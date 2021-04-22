using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.DTOs
{
    public class FileCreateDto
    {
        [Required]
        public string FileName { get; set; }
        [Required]
        public string Data { get; set; }

        [Required]
        public bool IsPhoto { get; set; }
        [Required]
        public string Extension { get; set; }
    }
}
