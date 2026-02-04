using SchoolManegementNew.Models;
using Dapper;
using System.Data;
namespace SchoolManegementNew.Repositories
{
    public class DashboardRepository:IDashboardRepository
    {
        private readonly IDbConnection _db;
        public DashboardRepository(IDbConnection db) {
            _db = db;
        }
        public DashboardViewModel GetDashboardCounts()
        {
            try
            {
                string query = @"
SELECT
(SELECT COUNT(*) FROM UserProfiles WHERE UserType='Teacher')AS TeacherCount,
SELECT COUNT(*) FROM UserProfiles WHERE UserType='Student')AS StudentCount,
SELECT COUNT(*) FROM Subjects)AS SubjectCount,
";
                return _db.QuerySingle<DashboardViewModel>(query);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
