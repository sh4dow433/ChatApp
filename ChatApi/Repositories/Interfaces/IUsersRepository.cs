using ChatApi.Models;

namespace ChatApi.Repositories.Interfaces
{
    public interface IUsersRepository : IRepository<AppUser>
    {
        AppUser GetByName(string name);
    }
}
