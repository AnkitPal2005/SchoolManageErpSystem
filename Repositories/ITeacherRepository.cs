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
        List<StudentSimpleViewModel> GetAllStudents(int subjectId);
        //void InsertMarks(SimpleMarksViewModel model, int subjectId);
        void InsertOrUpdateMarks(SimpleMarksViewModel model, int subjectId);
        SimpleMarksViewModel GetStudentMarks(string studentUserId, int subjectId);

        public int GetTeacherSubjectId(string teacherUserId);
          Task<PaginationViewModel<TeacherListViewModel>> GetTeachersPagedAsync(int pageNumber, int pageSize, string? search);
    }

}
