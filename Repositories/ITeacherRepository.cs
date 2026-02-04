using SchoolManegementNew.Models;
namespace SchoolManegementNew.Repositories
{
    public interface ITeacherRepository
    {
        List<TeacherListViewModel> GetAllTeachers();
        public void DeleteTeacher(string userId);
        TeacherEditViewModel GetTeacherById(string userId);
        void UpdateTeacher(TeacherEditViewModel model);

    }
}
