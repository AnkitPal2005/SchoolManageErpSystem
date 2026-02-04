using SchoolManegementNew.Models;
using System.Collections.Generic;
namespace SchoolManegementNew.Repositories
{
    public interface IStudentRepository
    {
        List<StudentListViewModel> GetAllStudents();
        void DeleteStudent(string UserId);
        StudentListViewModel GetStudentByUserId(string UserId);
        public void UpdateStudent(StudentListViewModel model);
    }
}
