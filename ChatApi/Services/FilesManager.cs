using ChatApi.DbAccess;
using ChatApi.Models;
using ChatApi.ServicesInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Services
{
    public class FilesManager : IFilesManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public FilesManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void DeleteFile(FileRecord fileRecord)
        {
            throw new NotImplementedException();
        }

        public FileModel GetFile(FileRecord fileRecord)
        {
            throw new NotImplementedException();
        }

        public FileRecord SaveFile(FileModel fileModel)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileModel.File.FileName);
            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                fileModel.File.CopyTo(stream);
            }
            FileRecord fileRecord = new FileRecord()
            {
                FileName = fileModel.File.FileName,
                IsPhoto = fileModel.IsPhoto,
                Path = path,
                Uploaded = DateTime.Now
            };
            _unitOfWork.Files.Insert(fileRecord);
            _unitOfWork.SaveChanges();
            return fileRecord;
        }
    }
}
