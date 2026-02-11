using SchoolManegementNew.Models;
using SchoolManegementNew.Models.Reports;

namespace SchoolManegementNew.Repositories.Reports
{
    public interface IAdminReportRepository
    {
        AdminReportSummary GetSummary();
        List<TeacherReportDto> GetTeachers();
        List<StudentReportDto> GetStudents();
        List<UserReportDto> GetUsers();
        List<StudentMarksReportDto> GetStudentMarks();
        public List<StudentListViewModel> GetStudentsBySearch(string? search);
        Task<PaginationViewModel<StudentListViewModel>>
 GetStudentsPagedAsync(int pageNumber, int pageSize, string? search);

        //public async Task<PaginationViewModel<StudentListViewModel>> GetStudentsPagedAsync(int pageNumber, int pageSize, string search);
    }
}
