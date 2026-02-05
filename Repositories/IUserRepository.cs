using SchoolManegementNew.Models;

namespace SchoolManegementNew.Repositories
{
    public interface IUserRepository
    {
        List<UserListViewModel> GetAllUsers();
    }
}
