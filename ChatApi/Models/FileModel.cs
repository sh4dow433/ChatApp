using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Models
{
    public class FileModel
    {
        public string UserId { get; set; }
        public bool IsPhoto { get; set; }
        public IFormFile File{ get; set; }
    }
}
