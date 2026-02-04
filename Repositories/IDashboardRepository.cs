using SchoolManegementNew.Models;

namespace SchoolManegementNew.Repositories
{
    public interface IDashboardRepository
    {
        DashboardViewModel GetDashboardCounts();
    }
}
