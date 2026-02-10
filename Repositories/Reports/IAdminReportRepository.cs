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
        public List<StudentListViewModel> GetStudentsBySearch(string? search,
    int pageNumber,
    int pageSize,
    out int totalRecords);

    }
}
