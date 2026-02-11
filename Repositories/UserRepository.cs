using Dapper;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using iText.StyledXmlParser.Jsoup.Select;
using Org.BouncyCastle.Utilities;
using SchoolManegementNew.Models;
using System.Data;

namespace SchoolManegementNew.Repositories
{
    public class UserRepository:IUserRepository
    {
        private readonly IDbConnection _db;
        public UserRepository(IDbConnection db) {
            _db = db;
        }
        //    public List<UserListViewModel> GetAllUsers()
        //    {
        //        string query = @"
        //SELECT 
        //    up.UserId,
        //    up.FullName,
        //    u.Email,
        //    up.PhoneNumber,
        //    up.UserType,
        //    s.Name AS SubjectName,
        //    up.RollNumber
        //FROM UserProfiles up
        //INNER JOIN AspNetUsers u ON u.Id = up.UserId
        //LEFT JOIN Subjects s ON s.Id = up.SubjectId
        //ORDER BY up.UserType";

        //        return _db.Query<UserListViewModel>(query).ToList();
        //    }
        public async Task<PaginationViewModel<UserListViewModel>> GetUserPagedAsync(int pageNumber, int pageSize, string? search,string?role,int?subjectId)
        {
            var offset = (pageNumber - 1) * pageSize;
            string query = @"SELECT COUNT(*)
FROM UserProfiles up
INNER JOIN AspNetUsers u ON u.Id = up.UserId
LEFT JOIN Subjects s ON s.Id = up.SubjectId
WHERE
(
    @Search IS NULL
    OR up.FullName LIKE '%' + @Search + '%'
    OR u.Email LIKE '%' + @Search + '%'
    
)

AND
(
    @Role IS NULL
    OR up.UserType = @Role

)
AND(
@SubjectId IS NULL
    OR up.SubjectId = @SubjectId
);

            SELECT
    up.UserId,
    up.FullName,
    u.Email,
    u.PhoneNumber,
    up.UserType,
    up.RollNumber,
    s.Name AS SubjectName
FROM UserProfiles up
INNER JOIN AspNetUsers u ON u.Id = up.UserId
LEFT JOIN Subjects s ON s.Id = up.SubjectId
WHERE
(
    @Search IS NULL
    OR up.FullName LIKE '%' + @Search + '%'
    OR u.Email LIKE '%' + @Search + '%'
  

)
AND
(
    @Role IS NULL
    OR up.UserType = @Role

)
AND(
@SubjectId IS NULL
    OR up.SubjectId = @SubjectId
)
ORDER BY up.FullName
OFFSET @Offset ROWS
FETCH NEXT @PageSize ROWS ONLY;
";

            using var multi = await _db.QueryMultipleAsync(query, new
            {
                Offset = offset,
                PageSize = pageSize,
                Search = search,
                Role = role,
                SubjectId = subjectId
            });
            var totalCount = await multi.ReadFirstAsync<int>();
            var users = await multi.ReadAsync<UserListViewModel>();
            return new PaginationViewModel<UserListViewModel>
            {
                Items = users,
                TotalCount = totalCount,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
        public async Task<List<string>> GetAllRolesAsync()
        {
            string query = @"SELECT DISTINCT UserType 
                     FROM UserProfiles
                     WHERE UserType IS NOT NULL";

            var roles = await _db.QueryAsync<string>(query);
            return roles.ToList();
        }
        public async Task<List<Subject>> GetAllSubjectsAsync()
        {
            string query = "SELECT Id, Name FROM Subjects ORDER BY Name";
            var subjects = await _db.QueryAsync<Subject>(query);
            return subjects.ToList();
        }

    }
}
