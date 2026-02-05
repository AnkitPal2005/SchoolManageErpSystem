using Dapper;
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
        public List<UserListViewModel> GetAllUsers()
        {
            string query = @"
    SELECT 
        up.UserId,
        up.FullName,
        u.Email,
        up.PhoneNumber,
        up.UserType,
        s.Name AS SubjectName,
        up.RollNumber
    FROM UserProfiles up
    INNER JOIN AspNetUsers u ON u.Id = up.UserId
    LEFT JOIN Subjects s ON s.Id = up.SubjectId
    ORDER BY up.UserType";

            return _db.Query<UserListViewModel>(query).ToList();
        }

    }
}
