//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
//using SchoolManegementNew.Repositories;
//using SchoolManegementNew.Models;
//using System.Security.Claims;
//namespace SchoolManegementNew.Controllers
//{
//    [Authorize(Roles = "Teacher")]
//    public class TeacherController : Controller
//    {
//        private readonly IDashboardRepository _dashboardRepo;
//        private readonly ITeacherRepository _teacherRepository;
//        public TeacherController(IDashboardRepository dashboardRepo, ITeacherRepository teacherRepository)
//        {
//            _dashboardRepo = dashboardRepo;
//            _teacherRepository = teacherRepository;
//        }
//        public IActionResult Dashboard()
//        {
//            return View();
//        }
//        [HttpGet]
//        public IActionResult GetDashboardCount()
//        {
//            try
//            {
//                var data = _dashboardRepo.GetDashboardCounts();
//                return Json(data);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { message = ex.Message });
//            }
//        }
//        [HttpGet]
//        public IActionResult Profile()
//        {
//            return View();
//        }
//        [HttpGet]
//        public IActionResult TeacherProfileCard()
//        {
//            try
//            {
//                string ?userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

//                var data = _teacherRepository.GetTeacherProfileById(userId);
//                return PartialView(data);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { message = ex.Message });
//            }
//        }
//        [HttpGet]
//        public IActionResult EditProfile(string id)
//        {
//            var model = _teacherRepository.GetTeacherProfileById(id);
//            return PartialView(model);
//        }
//        [HttpPost]
//        public IActionResult EditProfile(TeacherProfileViewModel model)
//        {
//            try
//            {
//                _teacherRepository.UpdateTeacherSelfProfile(model);
//                return Json(new { success = true, message = "Updated Profile Successfully" });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { error = ex.Message });
//            }
//        }
//        //private int GetLoggedInTeacherSubjectId()
//        //{
//        //    string teacherUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
//        //    return _teacherRepository.GetTeacherSubjectId(teacherUserId);
//        //}
//        public IActionResult Marks()
//        {
//            return View();  
//        }
//        public IActionResult StudentMarksPartial()
//        {
//            string teacherUserId =
//        User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";

//            int subjectId = _teacherRepository.GetTeacherSubjectId(teacherUserId);
//            var students = _teacherRepository.GetAllStudents(subjectId);
//            return PartialView(students);
//        }
//        [HttpGet]
//        public IActionResult AssignMarks(string StudentUserId)
//        {
//            string teacherUserId =
//        User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";

//            int subjectId = _teacherRepository.GetTeacherSubjectId(teacherUserId);

//            var model = _teacherRepository.GetStudentMarks(StudentUserId, subjectId);
//            return PartialView("AssignMarks",model);
//        }
//        [HttpPost]
//        public IActionResult SaveMarks(SimpleMarksViewModel model)
//        {
//            string teacherUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? ""; 
//            int subjectId = _teacherRepository.GetTeacherSubjectId(teacherUserId);
//            _teacherRepository.InsertOrUpdateMarks(model, subjectId);
//            return Json(new { success = true, message = "Marks saved Successfully" });
//        }
//    }
//}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManegementNew.Models;
using SchoolManegementNew.Repositories;
using System.Security.Claims;

namespace SchoolManegementNew.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        // =========================
        // DEPENDENCIES
        // =========================

        private readonly IDashboardRepository _dashboardRepo;
        private readonly ITeacherRepository _teacherRepository;

        public TeacherController(
            IDashboardRepository dashboardRepo,
            ITeacherRepository teacherRepository)
        {
            _dashboardRepo = dashboardRepo;
            _teacherRepository = teacherRepository;
        }

        // =========================
        // DASHBOARD
        // =========================

        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetDashboardCount()
        {
            try
            {
                var data = _dashboardRepo.GetDashboardCounts();
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        // =========================
        // PROFILE
        // =========================

        [HttpGet]
        public IActionResult Profile()
        {
            return View();
        }

        /// <summary>
        /// Loads teacher profile card using logged-in teacher's UserId
        /// Used for AJAX-based profile rendering
        /// </summary>
        [HttpGet]
        public IActionResult TeacherProfileCard()
        {
            try
            {
                string? userId = GetLoggedInUserId();
                var data = _teacherRepository.GetTeacherProfileById(userId);
                return PartialView(data);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult EditProfile(string id)
        {
            var model = _teacherRepository.GetTeacherProfileById(id);
            return PartialView(model);
        }

        [HttpPost]
        public IActionResult EditProfile(TeacherProfileViewModel model)
        {
            try
            {
                _teacherRepository.UpdateTeacherSelfProfile(model);
                return Json(new { success = true, message = "Updated Profile Successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // =========================
        // MARKS
        // =========================

        public IActionResult Marks()
        {
            return View();
        }

        /// <summary>
        /// Loads all students with marks for the logged-in teacher's subject
        /// Used to ensure teacher sees only their subject's students/marks
        /// </summary>
        public IActionResult StudentMarksPartial()
        {
            string teacherUserId = GetLoggedInUserId();
            int subjectId = _teacherRepository.GetTeacherSubjectId(teacherUserId);

            var students = _teacherRepository.GetAllStudents(subjectId);
            return PartialView(students);
        }

        /// <summary>
        /// Opens Assign / Update Marks modal
        /// If marks exist, they are pre-filled
        /// </summary>
        [HttpGet]
        public IActionResult AssignMarks(string studentUserId)
        {
            string teacherUserId = GetLoggedInUserId();
            int subjectId = _teacherRepository.GetTeacherSubjectId(teacherUserId);

            var model = _teacherRepository.GetStudentMarks(studentUserId, subjectId);
            return PartialView("AssignMarks", model);
        }

        /// <summary>
        /// Inserts or updates marks for the logged-in teacher's subject
        /// Ensures teacher cannot touch other subject marks
        /// </summary>
        [HttpPost]
        public IActionResult SaveMarks(SimpleMarksViewModel model)
        {
            string teacherUserId = GetLoggedInUserId();
            int subjectId = _teacherRepository.GetTeacherSubjectId(teacherUserId);

            _teacherRepository.InsertOrUpdateMarks(model, subjectId);

            return Json(new { success = true, message = "Marks saved Successfully" });
        }

        // =========================
        // HELPERS
        // =========================

        /// <summary>
        /// Returns logged-in user's UserId from claims
        /// Centralized to avoid duplicate code
        /// </summary>
        private string GetLoggedInUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        }
    }
}
