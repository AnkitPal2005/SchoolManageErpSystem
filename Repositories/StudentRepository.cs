//using Dapper;
//using System.Data;
//using SchoolManegementNew.Models;
//using System.Linq;
//using System.Collections.Generic;
//namespace SchoolManegementNew.Repositories
//{
//    public class StudentRepository : IStudentRepository
//    {
//        private readonly IDbConnection _db;
//        public StudentRepository(IDbConnection db)
//        {
//            _db = db;
//        }
//        //public List<StudentListViewModel> GetAllStudents()
//        //{
//        //    try
//        //    {
//        //        string sql = @"Select up.FullName,u.Email,up.PhoneNumber,up.RollNumber From UserProfiles up Inner Join AspNetUsers u On up.UserId=u.Id Where up.UserType='Student'";
//        //        return _db.Query<StudentListViewModel>(sql).ToList();
//        //    }
//        //    catch
//        //    {
//        //        throw;
//        //    }
//        //}
//        public List<StudentListViewModel> GetAllStudents()
//        {
//            try
//            {
//                string sql = @"SELECT up.UserId, up.FullName, u.Email, up.PhoneNumber, up.RollNumber 
//                      FROM UserProfiles up 
//                      INNER JOIN AspNetUsers u ON up.UserId = u.Id 
//                      WHERE up.UserType = 'Student'";
//                return _db.Query<StudentListViewModel>(sql).ToList();
//            }
//            catch
//            {
//                throw;
//            }
//        }
//        public void DeleteStudent(string UserId)
//        {
//            _db.Execute("DELETE FROM UserProfiles WHERE UserId=@Id", new { Id = UserId });
//            _db.Execute("DELETE FROM AspNetUsers WHERE Id=@Id", new { Id = UserId });
//        }
//        public StudentListViewModel GetStudentByUserId(string userId)
//        {
//            string query = @"
//                SELECT 
//                    up.UserId,
//                    up.FullName,
//                    u.Email,
//                    up.PhoneNumber,
//                    up.RollNumber
//                FROM UserProfiles up
//                INNER JOIN AspNetUsers u ON up.UserId = u.Id
//                WHERE up.UserId = @Id";

//            return _db.QueryFirstOrDefault<StudentListViewModel>(
//                query,
//                new { Id = userId });
//        }
//        public void UpdateStudent(StudentListViewModel model)
//        {
//            string query = @"
//    UPDATE UserProfiles
//    SET FullName=@Name,
//        PhoneNumber=@Phone,
//        RollNumber=@Roll
//    WHERE UserId=@Id";

//            _db.Execute(query, new
//            {
//                Name = model.FullName,
//                Phone = model.PhoneNumber,
//                Roll = model.RollNumber,
//                Id = model.UserId
//            });
//        }


//    }
//}
using Dapper;
using SchoolManegementNew.Models;
using System.Collections.Generic;
using System.Data;

namespace SchoolManegementNew.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        // =========================
        // DEPENDENCY
        // =========================

        private readonly IDbConnection _db;

        public StudentRepository(IDbConnection db)
        {
            _db = db;
        }

        // =========================
        // READ OPERATIONS
        // =========================

        /// <summary>
        /// Returns all students with basic details
        /// Used in Admin → See All Students
        /// </summary>
        public List<StudentListViewModel> GetAllStudents()
        {
            try
            {
                string sql = @"
                    SELECT 
                        up.UserId,
                        up.FullName,
                        u.Email,
                        up.PhoneNumber,
                        up.RollNumber
                    FROM UserProfiles up
                    INNER JOIN AspNetUsers u ON up.UserId = u.Id
                    WHERE up.UserType = 'Student'";

                return _db.Query<StudentListViewModel>(sql).ToList();
            }
            catch
            {
                // Let controller handle the exception
                throw;
            }
        }

        /// <summary>
        /// Returns a single student by UserId
        /// Used while editing student details
        /// </summary>
        public StudentListViewModel GetStudentByUserId(string userId)
        {
            string query = @"
                SELECT 
                    up.UserId,
                    up.FullName,
                    u.Email,
                    up.PhoneNumber,
                    up.RollNumber
                FROM UserProfiles up
                INNER JOIN AspNetUsers u ON up.UserId = u.Id
                WHERE up.UserId = @Id";

            return _db.QueryFirstOrDefault<StudentListViewModel>(
                query,
                new { Id = userId });
        }

        // =========================
        // WRITE OPERATIONS
        // =========================

        /// <summary>
        /// Updates student profile details
        /// Does not touch authentication data
        /// </summary>
        public void UpdateStudent(StudentListViewModel model)
        {
            string query = @"
                UPDATE UserProfiles
                SET FullName   = @Name,
                    PhoneNumber = @Phone,
                    RollNumber  = @Roll
                WHERE UserId = @Id";

            _db.Execute(query, new
            {
                Name = model.FullName,
                Phone = model.PhoneNumber,
                Roll = model.RollNumber,
                Id = model.UserId
            });
        }

        /// <summary>
        /// Deletes student from both UserProfiles and AspNetUsers
        /// Used by Admin only
        /// </summary>
        public void DeleteStudent(string userId)
        {
            _db.Execute("DELETE FROM UserProfiles WHERE UserId = @Id", new { Id = userId });
            _db.Execute("DELETE FROM AspNetUsers WHERE Id = @Id", new { Id = userId });
        }
    }
}
