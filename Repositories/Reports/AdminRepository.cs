using Dapper;
using iText.StyledXmlParser.Jsoup.Select;
using Microsoft.Identity.Client;
using SchoolManegementNew.Models.Reports;
using System.Data;

namespace SchoolManegementNew.Repositories.Reports
{
    public class AdminRepository:IAdminReportRepository
    {
        private readonly IDbConnection _db;
        public AdminRepository(IDbConnection db) { 
            _db = db;
        }
        //SummaryCount
        public AdminReportSummary GetSummary()
        {
            try
            {
                string query = @"SELECT
            COUNT(CASE WHEN UserType = 'Teacher' THEN 1 END) AS TotalTeachers,
            COUNT(CASE WHEN UserType = 'Student' THEN 1 END) AS TotalStudents,
            (SELECT COUNT(*) FROM Subjects) AS TotalSubjects,
            COUNT(*) AS TotalUsers
        FROM UserProfiles
    ";
                return _db.QuerySingle<AdminReportSummary>(query);

            }
            catch{
                throw;
            }
            
        }
        public List<TeacherReportDto> GetTeachers()
        {
            try
            {
                string query = @"
        SELECT 
            up.FullName AS Name,
            u.Email,
            ISNULL(s.Name, '-') AS SubjectName,
            CASE 
                WHEN up.SubjectId IS NULL THEN 'Not Assigned'
                ELSE 'Assigned'
            END AS Status
        FROM UserProfiles up
        INNER JOIN AspNetUsers u ON u.Id = up.UserId
        LEFT JOIN Subjects s ON s.Id = up.SubjectId
        WHERE up.UserType = 'Teacher'
        ORDER BY up.FullName
    ";
                return _db.Query<TeacherReportDto>(query).ToList();
            }
            catch { throw; }
        }
        public List<StudentReportDto> GetStudents()
        {
            try
            {
                string query = @"
                SELECT
                    up.RollNumber,
                    up.FullName AS Name,
                    u.Email,
                    COUNT(ist.SubjectId) AS TotalSubjects
                FROM UserProfiles up
                INNER JOIN AspNetUsers u ON u.Id = up.UserId
                LEFT JOIN IntermediateStudentTable ist 
                    ON ist.StudentUserId = up.UserId
                WHERE up.UserType = 'Student'
                GROUP BY up.RollNumber, up.FullName, u.Email
                ORDER BY up.RollNumber
            ";
                return _db.Query<StudentReportDto>(query).ToList();
            }
            catch {
                throw;
            }
        }
        public List<UserReportDto> GetUsers()
        {
            try
            {
                string query = @"
                SELECT
                    up.FullName AS Name,
                    u.Email,
                    up.UserType AS Role
                FROM UserProfiles up
                LEFT JOIN AspNetUsers u ON u.Id = up.UserId
                ORDER BY up.UserType, up.FullName
            ";


                return _db.Query<UserReportDto>(query).ToList();

            }
               
            
            catch
            {
                throw;
            }
        }

    }
}
