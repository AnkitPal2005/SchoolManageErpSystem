using Dapper;
using System.Data;
using SchoolManegementNew.Models;
namespace SchoolManegementNew.Repositories
{
    public class TeacherRepository:ITeacherRepository
    {
        private readonly IDbConnection _db;
        public TeacherRepository(IDbConnection db)
        {
            _db = db;
        }
        public List<TeacherListViewModel> GetAllTeachers()
        {
            try
            {
                //string query = @"Select up.FullName,u.Email,up.PhoneNumber,s.Name As SubjectName From UserProfiles up Inner Join AspNetUsers u on up.UserId=u.Id Left join Subjects s on up.SubjectId=s.Id where up.UserType='Teacher'";
                string query = @"Select 
up.UserId,       
up.FullName,
u.Email,
up.PhoneNumber,
up.SubjectId,    
s.Name As SubjectName 
From UserProfiles up 
Inner Join AspNetUsers u on up.UserId=u.Id 
Left join Subjects s on up.SubjectId=s.Id 
where up.UserType='Teacher'
";
                return _db.Query<TeacherListViewModel>(query).ToList();
                
               
            }
            catch
            {
                throw;
            }
        }
        public TeacherEditViewModel GetTeacherById(string userId)
        {
            string query = @"
    SELECT 
        up.UserId,
        up.FullName,
        up.PhoneNumber,
        up.SubjectId
    FROM UserProfiles up
    WHERE up.UserId=@Id";

            return _db.QueryFirstOrDefault<TeacherEditViewModel>(
                query,
                new { Id = userId });
        }

        public void DeleteTeacher(string userId)
        {
            string query1 = "DELETE FROM UserProfiles WHERE UserId=@Id";
            string query2 = "DELETE FROM AspNetUsers WHERE Id=@Id";

            _db.Execute(query1, new { Id = userId });
            _db.Execute(query2, new { Id = userId });
        }
        public void UpdateTeacher(TeacherEditViewModel model)
        {
            string query = @"
    UPDATE UserProfiles
    SET FullName=@Name,
        PhoneNumber=@Phone,
        SubjectId=@Subject
    WHERE UserId=@Id";

            _db.Execute(query, new
            {
                Name = model.FullName,
                Phone = model.PhoneNumber,
                Subject = model.SubjectId,
                Id = model.UserId
            });
        }

    }
}
