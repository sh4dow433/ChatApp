using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.DTOs
{
    public class FileRecordReadDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public bool IsPhoto { get; set; }
        public string FileLocation { get; set; }
        public DateTime Uploaded { get; set; }
    }
}
