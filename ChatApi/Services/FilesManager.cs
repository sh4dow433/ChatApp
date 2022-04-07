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
            string fileName = Guid.NewGuid().ToString() + StringSanitizer.MakeValidFileName(fileModel.File.FileName);
            string fileLocation = "files\\";
            if (fileModel.IsPhoto)
            {
                fileLocation = "files\\images\\";
            }
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileLocation, fileName);
            using (Stream stream = new FileStream(fullPath, FileMode.Create))
            {
                fileModel.File.CopyTo(stream);
            }
            FileRecord fileRecord = new FileRecord()
            {
                FileName = fileModel.File.FileName,
                IsPhoto = fileModel.IsPhoto,
                FileLocation = fileLocation + fileName,
                Uploaded = DateTime.Now
            };
            _unitOfWork.Files.Insert(fileRecord);
            _unitOfWork.SaveChanges();
            return fileRecord;
        }
    }
}
