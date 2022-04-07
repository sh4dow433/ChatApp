using AutoMapper;
using ChatApi.DTOs;
using ChatApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Profiles
{
    public class FileProfile : Profile
    {
        public FileProfile()
        {
            CreateMap<FileRecord, FileRecordReadDto>();
        }
    }
}
