using SchoolManegementNew.Models;

namespace SchoolManegementNew.Repositories
{
    public interface IUserRepository
    {
        //List<UserListViewModel> GetAllUsers();
        Task<PaginationViewModel<UserListViewModel>> GetUserPagedAsync(int pageNumber, int pageSize,  string? search,string? role, int? subjectId);
         Task<List<string>> GetAllRolesAsync();
        Task<List<Subject>> GetAllSubjectsAsync();
    }
}
