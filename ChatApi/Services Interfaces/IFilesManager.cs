using ChatApi.Models;

namespace ChatApi.ServicesInterfaces
{
    public interface IFilesManager
    {
        RawFile GetFile(FileRecord fileRecord);
        FileRecord SaveFile(RawFile file);
        bool DeleteFile(FileRecord fileRecord);
    }
}
