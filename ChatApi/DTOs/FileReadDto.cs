using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.DTOs
{
    public class FileReadDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public bool IsPhoto { get; set; }
        public string Extension { get; set; }
        public DateTime Uploaded { get; set; }
    }
}
