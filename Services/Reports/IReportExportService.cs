using SchoolManegementNew.Models;

namespace SchoolManegementNew.Services.Reports
{
    public interface IReportExportService
    {
        public byte[] GenerateExcelReport();
        byte[] GenerateFilteredUsersExcel(List<UserListViewModel> users);
    }
}
