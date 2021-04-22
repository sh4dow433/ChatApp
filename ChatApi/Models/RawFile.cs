using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Models
{
    public class RawFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Data { get; set; }

        public bool IsPhoto { get; set; }
        public string Extension { get; set; }
    }
}
