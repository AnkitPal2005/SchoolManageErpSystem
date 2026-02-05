using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SchoolManegementNew.Repositories;
using SchoolManegementNew.Models;
namespace SchoolManegementNew.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly IDashboardRepository _dashboardRepo;
        private readonly ITeacherRepository _teacherRepository;
        public TeacherController(IDashboardRepository dashboardRepo, ITeacherRepository teacherRepository)
        {
            _dashboardRepo = dashboardRepo;
            _teacherRepository = teacherRepository;
        }
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
        [HttpGet]
        public IActionResult Profile()
        {
            return View();
        }
        [HttpGet]
        public IActionResult TeacherProfileCard()
        {
            try
            {
                string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

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
    }
}
