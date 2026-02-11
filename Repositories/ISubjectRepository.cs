using SchoolManegementNew.Models;
using System.Collections.Generic;
namespace SchoolManegementNew.Repositories
{
    public interface ISubjectRepository
    {
        List<Subject> GetAllSubjects();
        bool AddSubject(string name);
        void InsertUserProfile(string userId, AddUserRequest model);
        //List<Subject> GetFreeSubjects();
        Subject GetSubjectById(int id);
        void UpdateSubject(int id, string name);
        void DeleteSubject(int id);
        List<Subject> GetFreeSubjects(string teacherId);

        public List<SubjectListViewModel> GetAllSubject();
        Task<PaginationViewModel<SubjectListViewModel>> GetSubjectsPagedAsync(int pageNumber, int pageSize, string? search);
    }
}
