//using Dapper;
//using System.Data;
//using SchoolManegementNew.Models;
//namespace SchoolManegementNew.Repositories
//{
//    public class TeacherRepository:ITeacherRepository
//    {
//        private readonly IDbConnection _db;
//        public TeacherRepository(IDbConnection db)
//        {
//            _db = db;
//        }
//        public List<TeacherListViewModel> GetAllTeachers()
//        {
//            try
//            {
//                //string query = @"Select up.FullName,u.Email,up.PhoneNumber,s.Name As SubjectName From UserProfiles up Inner Join AspNetUsers u on up.UserId=u.Id Left join Subjects s on up.SubjectId=s.Id where up.UserType='Teacher'";
//                string query = @"Select 
//up.UserId,       
//up.FullName,
//u.Email,
//up.PhoneNumber,
//up.SubjectId,    
//s.Name As SubjectName 
//From UserProfiles up 
//Inner Join AspNetUsers u on up.UserId=u.Id 
//Left join Subjects s on up.SubjectId=s.Id 
//where up.UserType='Teacher'
//";
//                return _db.Query<TeacherListViewModel>(query).ToList();


//            }
//            catch
//            {
//                throw;
//            }
//        }
//        public TeacherEditViewModel GetTeacherById(string userId)
//        {
//            string query = @"
//    SELECT 
//        up.UserId,
//        up.FullName,
//        up.PhoneNumber,
//        up.SubjectId
//    FROM UserProfiles up
//    WHERE up.UserId=@Id";

//            return _db.QueryFirstOrDefault<TeacherEditViewModel>(
//                query,
//                new { Id = userId });
//        }

//        public void DeleteTeacher(string userId)
//        {
//            string query1 = "DELETE FROM UserProfiles WHERE UserId=@Id";
//            string query2 = "DELETE FROM AspNetUsers WHERE Id=@Id";

//            _db.Execute(query1, new { Id = userId });
//            _db.Execute(query2, new { Id = userId });
//        }
//        public void UpdateTeacher(TeacherEditViewModel model)
//        {
//            string query = @"
//    UPDATE UserProfiles
//    SET FullName=@Name,
//        PhoneNumber=@Phone,
//        SubjectId=@Subject
//    WHERE UserId=@Id";

//            _db.Execute(query, new
//            {
//                Name = model.FullName,
//                Phone = model.PhoneNumber,
//                Subject = model.SubjectId,
//                Id = model.UserId
//            });
//        }
//        public TeacherProfileViewModel GetTeacherProfileById(string userId)
//        {
//            string query = @"
//    SELECT 
//           up.UserId,    
//        up.FullName,
//        u.Email,
//        up.PhoneNumber,
//        ISNULL(s.Name, 'Not Assigned') AS SubjectName
//    FROM UserProfiles up
//    INNER JOIN AspNetUsers u ON u.Id = up.UserId
//    LEFT JOIN Subjects s ON s.Id = up.SubjectId
//    WHERE up.UserId = @Id";

//            return _db.QueryFirstOrDefault<TeacherProfileViewModel>(
//                query,
//                new { Id = userId }
//            );
//        }
//        public bool UpdateTeacherSelfProfile(TeacherProfileViewModel model)
//        {
//            string query = @"
//    UPDATE UserProfiles
//    SET FullName = @Name,
//        PhoneNumber = @Phone
//    WHERE UserId = @Id";

//            int rows = _db.Execute(query, new
//            {
//                Name = model.FullName,
//                Phone = model.PhoneNumber,
//                Id = model.UserId
//            });

//            return rows > 0;
//        }
//        public int GetTeacherSubjectId(string teacherUserId)
//        {
//            string query = @"Select SubjectId From UserProfiles Where UserId=@UserId And UserType='Teacher'";
//            return _db.QuerySingle<int>(query, new { UserId = teacherUserId });
//        }
//        //public void InsertMarks(SimpleMarksViewModel model, int subjectId)
//        //{
//        //    string query = @"Insert Into IntermediateStudentTable (StudentUserId,SubjectId,MarksObtained,MaxMarks)Values(@StudentUserId,@SubjectId,@MarksObtained,@MaxMarks)";
//        //    _db.Execute(query, new
//        //    {
//        //        StudentUserId = model.StudentUserId,
//        //        SubjectId = subjectId,
//        //        MarksObtained = model.MarksObtained,
//        //        MaxMarks = model.MaxMarks,
//        //    });
//        //}
//        public List<StudentSimpleViewModel> GetAllStudents(int subjectId)
//        {
//            string query = @"
//        SELECT 
//            u.Id AS StudentUserId,
//            up.FullName,
//            up.RollNumber,
// s.Name AS SubjectName,
//            m.MarksObtained,
//            m.MaxMarks,
// m.StudentUserId AS MarksRowExists  
//        FROM UserProfiles up
//        INNER JOIN AspNetUsers u ON u.Id = up.UserId
//        Left join IntermediateStudentTable m on m.StudentUserId=u.Id
//         AND m.SubjectId = @SubjectId
//INNER JOIN Subjects s 
//    ON s.Id = @SubjectId
//        WHERE up.UserType = 'Student'";

//            return _db.Query<StudentSimpleViewModel>(query, new { SubjectId = subjectId }).ToList();
//        }
//        public void InsertOrUpdateMarks(SimpleMarksViewModel model, int subjectId)
//        {
//            string query = @"
//        IF EXISTS (
//            SELECT 1 
//            FROM IntermediateStudentTable
//            WHERE StudentUserId = @StudentUserId
//              AND SubjectId = @SubjectId
//        )
//        BEGIN
//            UPDATE IntermediateStudentTable
//            SET MarksObtained = @MarksObtained,
//                MaxMarks = @MaxMarks
//            WHERE StudentUserId = @StudentUserId
//              AND SubjectId = @SubjectId
//        END
//        ELSE
//        BEGIN
//            INSERT INTO IntermediateStudentTable
//            (StudentUserId, SubjectId, MarksObtained, MaxMarks)
//            VALUES
//            (@StudentUserId, @SubjectId, @MarksObtained, @MaxMarks)
//        END";

//            _db.Execute(query, new
//            {
//                StudentUserId = model.StudentUserId,
//                SubjectId = subjectId,
//                MarksObtained = model.MarksObtained,
//                MaxMarks = model.MaxMarks
//            });
//        }
//        public SimpleMarksViewModel GetStudentMarks(string studentUserId, int subjectId)
//        {
//            string query = @"
//        SELECT 
//            StudentUserId,
//            MarksObtained,
//            MaxMarks
//        FROM IntermediateStudentTable
//        WHERE StudentUserId = @StudentUserId
//          AND SubjectId = @SubjectId";

//            var data = _db.QueryFirstOrDefault<SimpleMarksViewModel>(
//                query,
//                new { StudentUserId = studentUserId, SubjectId = subjectId }
//            );


//            if (data == null)
//            {
//                return new SimpleMarksViewModel
//                {
//                    StudentUserId = studentUserId
//                };
//            }

//            return data;
//        }

//    }
//}
using Dapper;
using DocumentFormat.OpenXml.Wordprocessing;
using SchoolManegementNew.Models;
using System.Collections.Generic;
using System.Data;

namespace SchoolManegementNew.Repositories
{
    public class TeacherRepository : ITeacherRepository
    {
        // =========================
        // DEPENDENCY
        // =========================

        private readonly IDbConnection _db;

        public TeacherRepository(IDbConnection db)
        {
            _db = db;
        }

        // =========================
        // TEACHER READ OPERATIONS (ADMIN SIDE)
        // =========================

        /// <summary>
        /// Returns all teachers with their assigned subject (if any)
        /// Used in Admin → See All Teachers
        /// </summary>
        public List<TeacherListViewModel> GetAllTeachers()
        {
            try
            {
                string query = @"
                    SELECT 
                        up.UserId,
                        up.FullName,
                        u.Email,
                        up.PhoneNumber,
                        up.SubjectId,
                        s.Name AS SubjectName
                    FROM UserProfiles up
                    INNER JOIN AspNetUsers u ON up.UserId = u.Id
                    LEFT JOIN Subjects s ON up.SubjectId = s.Id
                    WHERE up.UserType = 'Teacher'";

                return _db.Query<TeacherListViewModel>(query).ToList();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Returns teacher basic data for edit modal (Admin)
        /// </summary>
        public TeacherEditViewModel GetTeacherById(string userId)
        {
            string query = @"
                SELECT 
                    up.UserId,
                    up.FullName,
                    up.PhoneNumber,
                    up.SubjectId
                FROM UserProfiles up
                WHERE up.UserId = @Id";

            return _db.QueryFirstOrDefault<TeacherEditViewModel>(
                query,
                new { Id = userId });
        }

        // =========================
        // TEACHER WRITE OPERATIONS (ADMIN SIDE)
        // =========================

        /// <summary>
        /// Deletes teacher from both UserProfiles and AspNetUsers
        /// </summary>
        public void DeleteTeacher(string userId)
        {
            string query1 = "DELETE FROM UserProfiles WHERE UserId = @Id";
            string query2 = "DELETE FROM AspNetUsers WHERE Id = @Id";

            _db.Execute(query1, new { Id = userId });
            _db.Execute(query2, new { Id = userId });
        }

        /// <summary>
        /// Updates teacher details including subject assignment (Admin)
        /// </summary>
        public void UpdateTeacher(TeacherEditViewModel model)
        {
            string query = @"
                UPDATE UserProfiles
                SET FullName = @Name,
                    PhoneNumber = @Phone,
                    SubjectId = @Subject
                WHERE UserId = @Id";

            _db.Execute(query, new
            {
                Name = model.FullName,
                Phone = model.PhoneNumber,
                Subject = model.SubjectId,
                Id = model.UserId
            });
        }

        // =========================
        // TEACHER SELF PROFILE (TEACHER SIDE)
        // =========================

        /// <summary>
        /// Returns teacher profile for Teacher → Profile page
        /// </summary>
        public TeacherProfileViewModel GetTeacherProfileById(string userId)
        {
            string query = @"
                SELECT 
                    up.UserId,
                    up.FullName,
                    u.Email,
                    up.PhoneNumber,
                    ISNULL(s.Name, 'Not Assigned') AS SubjectName
                FROM UserProfiles up
                INNER JOIN AspNetUsers u ON u.Id = up.UserId
                LEFT JOIN Subjects s ON s.Id = up.SubjectId
                WHERE up.UserId = @Id";

            return _db.QueryFirstOrDefault<TeacherProfileViewModel>(
                query,
                new { Id = userId });
        }

        /// <summary>
        /// Updates teacher own profile (Name & Phone only)
        /// </summary>
        public bool UpdateTeacherSelfProfile(TeacherProfileViewModel model)
        {
            string query = @"
                UPDATE UserProfiles
                SET FullName = @Name,
                    PhoneNumber = @Phone
                WHERE UserId = @Id";

            int rows = _db.Execute(query, new
            {
                Name = model.FullName,
                Phone = model.PhoneNumber,
                Id = model.UserId
            });

            return rows > 0;
        }

        /// <summary>
        /// Returns subject id assigned to logged-in teacher
        /// Used in Marks module
        /// </summary>
        public int GetTeacherSubjectId(string teacherUserId)
        {
            string query = @"
                SELECT SubjectId 
                FROM UserProfiles 
                WHERE UserId = @UserId 
                  AND UserType = 'Teacher'";

            return _db.QuerySingle<int>(query, new { UserId = teacherUserId });
        }

        // =========================
        // STUDENT MARKS (TEACHER SIDE)
        // =========================

        /// <summary>
        /// Returns students list with marks for teacher's subject
        /// Used in Teacher → Marks page
        /// </summary>
        public List<StudentSimpleViewModel> GetAllStudents(int subjectId)
        {
            string query = @"
                SELECT 
                    u.Id AS StudentUserId,
                    up.FullName,
                    up.RollNumber,
                    s.Name AS SubjectName,
                    m.MarksObtained,
                    m.MaxMarks,
                    m.StudentUserId AS MarksRowExists
                FROM UserProfiles up
                INNER JOIN AspNetUsers u ON u.Id = up.UserId
                LEFT JOIN IntermediateStudentTable m 
                    ON m.StudentUserId = u.Id
                   AND m.SubjectId = @SubjectId
                INNER JOIN Subjects s 
                    ON s.Id = @SubjectId
                WHERE up.UserType = 'Student'";

            return _db.Query<StudentSimpleViewModel>(query, new { SubjectId = subjectId }).ToList();
        }

        /// <summary>
        /// Inserts or updates marks for a student (UPSERT logic)
        /// </summary>
        public void InsertOrUpdateMarks(SimpleMarksViewModel model, int subjectId)
        {
            string query = @"
                IF EXISTS (
                    SELECT 1 
                    FROM IntermediateStudentTable
                    WHERE StudentUserId = @StudentUserId
                      AND SubjectId = @SubjectId
                )
                BEGIN
                    UPDATE IntermediateStudentTable
                    SET MarksObtained = @MarksObtained,
                        MaxMarks = @MaxMarks
                    WHERE StudentUserId = @StudentUserId
                      AND SubjectId = @SubjectId
                END
                ELSE
                BEGIN
                    INSERT INTO IntermediateStudentTable
                    (StudentUserId, SubjectId, MarksObtained, MaxMarks)
                    VALUES
                    (@StudentUserId, @SubjectId, @MarksObtained, @MaxMarks)
                END";

            _db.Execute(query, new
            {
                StudentUserId = model.StudentUserId,
                SubjectId = subjectId,
                MarksObtained = model.MarksObtained,
                MaxMarks = model.MaxMarks
            });
        }

        /// <summary>
        /// Returns marks for a specific student & subject
        /// Used while opening Assign / Update Marks modal
        /// </summary>
        public SimpleMarksViewModel GetStudentMarks(string studentUserId, int subjectId)
        {
            string query = @"
                SELECT 
                    StudentUserId,
                    MarksObtained,
                    MaxMarks
                FROM IntermediateStudentTable
                WHERE StudentUserId = @StudentUserId
                  AND SubjectId = @SubjectId";

            var data = _db.QueryFirstOrDefault<SimpleMarksViewModel>(
                query,
                new { StudentUserId = studentUserId, SubjectId = subjectId });

            // If marks not assigned yet, return empty model with StudentUserId
            if (data == null)
            {
                return new SimpleMarksViewModel
                {
                    StudentUserId = studentUserId
                };
            }

            return data;
        }
        public async Task<PaginationViewModel<TeacherListViewModel>> GetTeachersPagedAsync(int pageNumber, int pageSize, string? search)

        {
            var offset = (pageNumber - 1) * pageSize;
            var query = @"
SELECT COUNT(*)
FROM UserProfiles up
INNER JOIN AspNetUsers u ON u.Id = up.UserId
LEFT JOIN Subjects s ON s.Id = up.SubjectId
WHERE up.UserType = 'Teacher'
AND (
    @Search IS NULL
    OR up.FullName LIKE '%' + @Search + '%'
    OR u.Email LIKE '%' + @Search + '%'
    OR s.Name LIKE '%' + @Search + '%'
);

SELECT 
    up.UserId,
    up.FullName,
    u.Email,
    up.PhoneNumber,
    up.SubjectId,
    s.Name AS SubjectName
FROM UserProfiles up
INNER JOIN AspNetUsers u ON u.Id = up.UserId
LEFT JOIN Subjects s ON s.Id = up.SubjectId
WHERE up.UserType = 'Teacher'
AND (
    @Search IS NULL
    OR up.FullName LIKE '%' + @Search + '%'
    OR u.Email LIKE '%' + @Search + '%'
    OR s.Name LIKE '%' + @Search + '%'
)
ORDER BY up.FullName
OFFSET @Offset ROWS
FETCH NEXT @PageSize ROWS ONLY;
";
            using var multi = await _db.QueryMultipleAsync(query, new
            {
                Offset = offset,
                PageSize = pageSize,
                Search = search
            });
            var totalCount=await multi.ReadFirstAsync<int>();
            var teachers = await multi.ReadAsync<TeacherListViewModel>();
            return new PaginationViewModel<TeacherListViewModel>
            {
                Items = teachers,
                TotalCount = totalCount,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

    }
}
