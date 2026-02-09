using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManegementNew.Models;
using SchoolManegementNew.Repositories;
using System.Security.Claims;
namespace SchoolManegementNew.Controllers
{
    [Authorize(Roles ="Student")]
    public class StudentController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        public StudentController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }
        //public IActionResult Dashboard()
        //{
        //    return View();
        //}
        public IActionResult Profile()
        {
            return View(); 
        }
        public IActionResult Marks()
        {
            return View(); 
        }
        [HttpGet]
        public IActionResult StudentProfileCard()
        {
            try
            {
                string? userId = GetLoggedInUserId();
                var data = _studentRepository.GetStudentByUserId(userId);
                return PartialView(data);
            }
            catch (Exception ex) { 
                return Json(new { message = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult EditProfile(string id)
        {
            var model=_studentRepository.GetStudentByUserId(id);
            return PartialView(model);
        }
        [HttpPost]
        public IActionResult UpdateProfile(StudentListViewModel model)
        {
            try
            {
                _studentRepository.UpdateStudentSelfProfile(model);
                return Json(new { success = true, message = "Update Profile Successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        private string GetLoggedInUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        }
        public IActionResult StudentMarksPartial()
        {
            string UserId = GetLoggedInUserId();
            //int subjectId = _teacherRepository.GetTeacherSubjectId(teacherUserId);

            var students = _studentRepository.GetStudentMarks(UserId);
            return PartialView(students);
        }
    }
}
