using ChatApi.Models;

namespace ChatApi.ServicesInterfaces
{
    public interface IFilesManager
    {
        FileModel GetFile(FileRecord fileRecord);
        FileRecord SaveFile(FileModel file);
        void DeleteFile(FileRecord fileRecord);
    }
}
