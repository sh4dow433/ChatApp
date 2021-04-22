using ChatApi.DbAccess;
using ChatApi.Models;
using ChatApi.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Repositories
{
    public class FilesRepository : BaseRepository<FileRecord>, IFilesRepository
    {
        public FilesRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
