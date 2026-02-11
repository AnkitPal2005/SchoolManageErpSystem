using Dapper;
using iText.StyledXmlParser.Jsoup.Select;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Schema;
using SchoolManegementNew.Models;
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
        public List<StudentMarksReportDto> GetStudentMarks()
        {
            string query = @"
                SELECT
                    up.RollNumber,
                    up.FullName AS StudentName,
                    s.Name AS SubjectName,
                    ist.MarksObtained,
                    ist.MaxMarks
                FROM IntermediateStudentTable ist
                INNER JOIN UserProfiles up 
                    ON up.UserId = ist.StudentUserId
                INNER JOIN Subjects s 
                    ON s.Id = ist.SubjectId
                ORDER BY up.RollNumber, s.Name
            ";


            return _db.Query<StudentMarksReportDto>(query).ToList();
        }
        public List<StudentListViewModel> GetStudentsBySearch(string? search)
        {
            string query = @"
SELECT 
    u.Id AS UserId,
    up.RollNumber,
    u.Email,
    up.FullName,
    u.PhoneNumber,
    ISNULL(SUM(ist.MarksObtained), 0) AS Marks,
    COUNT(ist.SubjectId) AS TotalSubjects
FROM UserProfiles up
INNER JOIN AspNetUsers u
    ON u.Id = up.UserId
LEFT JOIN IntermediateStudentTable ist
    ON ist.StudentUserId = up.UserId
WHERE up.UserType = 'Student'
    AND (
        @Search IS NULL
        OR up.FullName LIKE '%' + @Search + '%'
        OR u.Email LIKE '%' + @Search + '%'
        OR up.RollNumber LIKE '%' + @Search + '%'
    )
GROUP BY u.Id, up.RollNumber, u.Email, up.FullName, u.PhoneNumber
ORDER BY up.RollNumber";

            return _db.Query<StudentListViewModel>(
                query,
                new { Search = search }
            ).ToList();
        }
        public async Task<PaginationViewModel<StudentListViewModel>>
 GetStudentsPagedAsync(int pageNumber, int pageSize, string? search)
        {
            var offset = (pageNumber - 1) * pageSize;

            var query = @"
    SELECT COUNT(*)
    FROM UserProfiles up
    INNER JOIN AspNetUsers u ON u.Id = up.UserId
    WHERE up.UserType = 'Student'
    AND (
        @Search IS NULL
        OR up.FullName LIKE '%' + @Search + '%'
        OR u.Email LIKE '%' + @Search + '%'
        OR up.RollNumber LIKE '%' + @Search + '%'
    );

    SELECT 
        u.Id AS UserId,
        up.FullName,
        u.Email,
        u.PhoneNumber,
        up.RollNumber,
        ISNULL(SUM(ist.MarksObtained),0) AS Marks,
        COUNT(ist.SubjectId) AS TotalSubjects
    FROM UserProfiles up
    INNER JOIN AspNetUsers u ON u.Id = up.UserId
    LEFT JOIN IntermediateStudentTable ist 
        ON ist.StudentUserId = up.UserId
    WHERE up.UserType = 'Student'
    AND (
        @Search IS NULL
        OR up.FullName LIKE '%' + @Search + '%'
        OR u.Email LIKE '%' + @Search + '%'
        OR up.RollNumber LIKE '%' + @Search + '%'
    )
    GROUP BY u.Id, up.FullName, u.Email, u.PhoneNumber, up.RollNumber
    ORDER BY up.RollNumber
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    ";

            using var multi = await _db.QueryMultipleAsync(query, new
            {
                Offset = offset,
                PageSize = pageSize,
                Search = search
            });

            var totalCount = await multi.ReadFirstAsync<int>();
            var students = await multi.ReadAsync<StudentListViewModel>();

            return new PaginationViewModel<StudentListViewModel>
            {
                Items = students,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

    }
}
