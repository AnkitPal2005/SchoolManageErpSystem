//using Dapper;
//using System.Data;
//using SchoolManegementNew.Models;
//using System.Linq;
//using System.Collections.Generic;
//namespace SchoolManegementNew.Repositories
//{
//    public class SubjectRepository: ISubjectRepository
//    {
//        private readonly IDbConnection _db;
//        public SubjectRepository(IDbConnection db)
//        {
//            _db = db;   
//        }
//        public List<Subject> GetAllSubjects()
//        {
//            string sql = @"Select * From Subjects Order by Name";
//            return _db.Query<Subject>(sql).ToList();
//        }
//        public bool AddSubject(string name)
//        {
//            try
//            {
//                string sql = @"Insert Into Subjects(Name)Values(@Name)";
//                _db.Execute(sql, new { Name = name });
//                return true;
//            }
//            catch
//            {
//                throw;
//            }
//        }
//        public void InsertUserProfile(string userId, AddUserRequest model)
//        {
//            try
//            {
//                string query = @"
//    INSERT INTO UserProfiles
//    (UserId, FullName, PhoneNumber, RollNumber, SubjectId, UserType)
//    VALUES
//    (@UserId, @FullName, @Phone, @Roll, @SubjectId, @Type)
//    ";

//                _db.Execute(query, new
//                {
//                    UserId = userId,
//                    FullName = model.FullName,
//                    Phone = model.PhoneNumber,
//                    Roll = model.RoleType == "Student" ? model.RollNumber : null,
//                    SubjectId = model.RoleType == "Teacher" ? model.SubjectId : null,
//                    Type = model.RoleType
//                });
//            }
//            catch
//            {
//                throw;
//            }
//        }
//        public List<Subject> GetFreeSubjects(string teacherId)
//        {
//            string query = @"
//    SELECT *
//    FROM Subjects
//    WHERE Id NOT IN
//    (
//        SELECT SubjectId
//        FROM UserProfiles
//        WHERE UserType = 'Teacher'
//        AND SubjectId IS NOT NULL
//        AND (UserId != @Id OR @Id Is NULL)
//    )";

//            return _db.Query<Subject>(query, new { Id = teacherId }).ToList();
//        }

//        public Subject GetSubjectById(int id)
//        {
//            return _db.QueryFirstOrDefault<Subject>(
//                "SELECT * FROM Subjects WHERE Id=@Id",
//                new { Id = id });
//        }

//        public void UpdateSubject(int id, string name)
//        {
//            string query = "UPDATE Subjects SET Name=@Name WHERE Id=@Id";
//            _db.Execute(query, new { Name = name, Id = id });
//        }

//        public void DeleteSubject(int id)
//        {
//            string query = "DELETE FROM Subjects WHERE Id=@Id";
//            _db.Execute(query, new { Id = id });
//        }

//        public List<SubjectListViewModel> GetAllSubject()
//        {
//            string query = @"
//        SELECT 
//            s.Id,
//            s.Name,
//            CASE 
//                WHEN EXISTS (
//                    SELECT 1 
//                    FROM UserProfiles up
//                    WHERE up.SubjectId = s.Id
//                      AND up.UserType = 'Teacher'
//                )
//                THEN 1
//                ELSE 0
//            END AS IsAssigned
//        FROM Subjects s";

//            return _db.Query<SubjectListViewModel>(query).ToList();
//        }
//    }
//}
using Dapper;
using SchoolManegementNew.Models;
using System.Collections.Generic;
using System.Data;

namespace SchoolManegementNew.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        // =========================
        // DEPENDENCY
        // =========================

        private readonly IDbConnection _db;

        public SubjectRepository(IDbConnection db)
        {
            _db = db;
        }

        // =========================
        // SUBJECT READ OPERATIONS
        // =========================

        /// <summary>
        /// Returns all subjects sorted by name
        /// Used in dropdowns and simple subject listings
        /// </summary>
        public List<Subject> GetAllSubjects()
        {
            string sql = @"SELECT * FROM Subjects ORDER BY Name";
            return _db.Query<Subject>(sql).ToList();
        }

        /// <summary>
        /// Returns a single subject by Id
        /// Used while editing subject
        /// </summary>
        public Subject GetSubjectById(int id)
        {
            return _db.QueryFirstOrDefault<Subject>(
                "SELECT * FROM Subjects WHERE Id = @Id",
                new { Id = id });
        }

        /// <summary>
        /// Returns subjects with assignment status
        /// Used in Admin → See All Subjects
        /// </summary>
        public List<SubjectListViewModel> GetAllSubject()
        {
            string query = @"
                SELECT 
                    s.Id,
                    s.Name,
                    CASE 
                        WHEN EXISTS (
                            SELECT 1 
                            FROM UserProfiles up
                            WHERE up.SubjectId = s.Id
                              AND up.UserType = 'Teacher'
                        )
                        THEN 1
                        ELSE 0
                    END AS IsAssigned
                FROM Subjects s";

            return _db.Query<SubjectListViewModel>(query).ToList();
        }

        /// <summary>
        /// Returns subjects which are not assigned to any teacher
        /// Used while assigning / editing teacher
        /// </summary>
        public List<Subject> GetFreeSubjects(string teacherId)
        {
            string query = @"
                SELECT *
                FROM Subjects
                WHERE Id NOT IN
                (
                    SELECT SubjectId
                    FROM UserProfiles
                    WHERE UserType = 'Teacher'
                      AND SubjectId IS NOT NULL
                      AND (UserId != @Id OR @Id IS NULL)
                )";

            return _db.Query<Subject>(query, new { Id = teacherId }).ToList();
        }

        // =========================
        // SUBJECT WRITE OPERATIONS
        // =========================

        /// <summary>
        /// Inserts a new subject
        /// Unique constraint handled at DB level
        /// </summary>
        public bool AddSubject(string name)
        {
            try
            {
                string sql = @"INSERT INTO Subjects (Name) VALUES (@Name)";
                _db.Execute(sql, new { Name = name });
                return true;
            }
            catch
            {
                // Exception handled at controller level
                throw;
            }
        }

        /// <summary>
        /// Updates subject name
        /// </summary>
        public void UpdateSubject(int id, string name)
        {
            string query = @"UPDATE Subjects SET Name = @Name WHERE Id = @Id";
            _db.Execute(query, new { Name = name, Id = id });
        }

        /// <summary>
        /// Deletes subject
        /// Will fail if subject is assigned to teacher (FK constraint)
        /// </summary>
        public void DeleteSubject(int id)
        {
            string query = @"DELETE FROM Subjects WHERE Id = @Id";
            _db.Execute(query, new { Id = id });
        }

        // =========================
        // USER PROFILE CREATION
        // =========================

        /// <summary>
        /// Inserts record into UserProfiles after Identity user creation
        /// Handles both Teacher and Student based on RoleType
        /// </summary>
        public void InsertUserProfile(string userId, AddUserRequest model)
        {
            try
            {
                string query = @"
                    INSERT INTO UserProfiles
                    (UserId, FullName, PhoneNumber, RollNumber, SubjectId, UserType)
                    VALUES
                    (@UserId, @FullName, @Phone, @Roll, @SubjectId, @Type)";

                _db.Execute(query, new
                {
                    UserId = userId,
                    FullName = model.FullName,
                    Phone = model.PhoneNumber,
                    Roll = model.RoleType == "Student" ? model.RollNumber : null,
                    SubjectId = model.RoleType == "Teacher" ? model.SubjectId : null,
                    Type = model.RoleType
                });
            }
            catch
            {
                // Let controller handle DB constraint messages
                throw;
            }
        }
    }
}

