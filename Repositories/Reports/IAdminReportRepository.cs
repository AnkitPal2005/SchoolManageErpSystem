using SchoolManegementNew.Models.Reports;
namespace SchoolManegementNew.Repositories.Reports
{
    public interface IAdminReportRepository
    {
        AdminReportSummary GetSummary();
        List<TeacherReportDto> GetTeachers();
        List<StudentReportDto> GetStudents();
        List<UserReportDto> GetUsers();
    }
}
