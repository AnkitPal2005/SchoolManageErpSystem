using Dapper;
using System.Data;
using SchoolManegementNew.Models;
using System.Linq;
using System.Collections.Generic;
namespace SchoolManegementNew.Repositories
{
    public class SubjectRepository: ISubjectRepository
    {
        private readonly IDbConnection _db;
        public SubjectRepository(IDbConnection db)
        {
            _db = db;   
        }
        public List<Subject> GetAllSubjects()
        {
            string sql = @"Select * From Subjects Order by Name";
            return _db.Query<Subject>(sql).ToList();
        }
        public bool AddSubject(string name)
        {
            try
            {
                string sql = @"Insert Into Subjects(Name)Values(@Name)";
                _db.Execute(sql, new { Name = name });
                return true;
            }
            catch
            {
                throw;
            }
        }
        public void InsertUserProfile(string userId, AddUserRequest model)
        {
            try
            {
                string query = @"
    INSERT INTO UserProfiles
    (UserId, FullName, PhoneNumber, RollNumber, SubjectId, UserType)
    VALUES
    (@UserId, @FullName, @Phone, @Roll, @SubjectId, @Type)
    ";

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
                throw;
            }
        }
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
        AND UserId != @Id
    )";

            return _db.Query<Subject>(query, new { Id = teacherId }).ToList();
        }

        public Subject GetSubjectById(int id)
        {
            return _db.QueryFirstOrDefault<Subject>(
                "SELECT * FROM Subjects WHERE Id=@Id",
                new { Id = id });
        }

        public void UpdateSubject(int id, string name)
        {
            string query = "UPDATE Subjects SET Name=@Name WHERE Id=@Id";
            _db.Execute(query, new { Name = name, Id = id });
        }

        public void DeleteSubject(int id)
        {
            string query = "DELETE FROM Subjects WHERE Id=@Id";
            _db.Execute(query, new { Id = id });
        }

        
    }
}
