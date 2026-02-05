using SchoolManegementNew.Models;
namespace SchoolManegementNew.Repositories
{
    public interface ITeacherRepository
    {
        List<TeacherListViewModel> GetAllTeachers();
        public void DeleteTeacher(string userId);
        TeacherEditViewModel GetTeacherById(string userId);
           TeacherProfileViewModel GetTeacherProfileById(string userId);
        public bool UpdateTeacherSelfProfile(TeacherProfileViewModel model);
        void UpdateTeacher(TeacherEditViewModel model);

    }
}
