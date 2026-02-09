//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
//using SchoolManegementNew.Repositories;
//using Microsoft.AspNetCore.Identity;
//using SchoolManegementNew.Models;
//using System.Globalization;
//namespace SchoolManegementNew.Controllers
//{
//    [Authorize(Roles = "Admin")]
//    public class AdminController : Controller
//    {
//        private readonly IDashboardRepository _dashboardRepo;
//        private readonly ITeacherRepository _teacherRepo;
//        private readonly IStudentRepository _studentRepo;
//        private readonly ISubjectRepository _subjectRepo;
//        private readonly IUserRepository _userRepo;
//        private readonly UserManager<IdentityUser> _userManager;
//        private readonly RoleManager<IdentityRole> _roleManager;
//        public AdminController(IDashboardRepository dashboardRepo, ITeacherRepository teacherRepo, IStudentRepository studentRepo, ISubjectRepository subjectRepo, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IUserRepository userRepo)
//        {
//            _teacherRepo = teacherRepo;
//            _dashboardRepo = dashboardRepo;
//            _studentRepo = studentRepo;
//            _subjectRepo = subjectRepo;
//            _roleManager = roleManager;
//            _userManager = userManager;
//            _userRepo = userRepo;
//        }


//        public IActionResult Dashboard()
//        {
//           return View();
//        }

//        [HttpGet]
//        public IActionResult GetDashboardCount()
//        {
//            try
//            {
//                var data = _dashboardRepo.GetDashboardCounts();
//                return Json(data);
//            }
//            catch (Exception ex) { 
//                return Json(new { message = ex.Message });
//            }
//        }
//        public IActionResult Teachers()
//        {
//            return View();
//        }
//        [HttpPost]
//        public IActionResult LoadTeachers()
//        {
//            try
//            {
//                var data = _teacherRepo.GetAllTeachers();
//                return PartialView(data);

//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }
//        public IActionResult Students()
//        {
//            return View();
//        }
//        [HttpPost]
//        public IActionResult LoadStudents()
//        {
//            try
//            {
//                var data = _studentRepo.GetAllStudents();
//                return PartialView(data);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }
//        [HttpGet]
//        public IActionResult Subjects()
//        {
//            return View();
//        }

//        // Return the subjects partial explicitly (used by the client)
//        [HttpGet]
//        public IActionResult LoadSubjects()
//        {
//            try
//            {
//                var data = _subjectRepo.GetAllSubject();
//                return PartialView("LoadSubjects", data);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }

//        // Provide a GET endpoint to return the "AddSubject" modal partial
//        [HttpGet]
//        public IActionResult AddSubjects()
//        {
//            try
//            {
//                return PartialView("AddSubject");
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }

//        [HttpPost]
//        public IActionResult AddSubject(string subjectName)
//        {
//            try
//            {
//                if (string.IsNullOrWhiteSpace(subjectName))
//                {
//                    return Json(new { success = false, message = "Subject name required" });
//                }
//                _subjectRepo.AddSubject(subjectName);
//                return Json(new { success = true, message = "Subject Added Successfully" });
//            }
//            catch (Exception ex)
//            {
//                if (ex.Message.Contains("UQ_Subject_Name"))
//                {
//                    return Json(new { success = false, message = "Subject Already Exist" });
//                }
//                return Json(new { success = false, message = "Subject Already exist" });

//            }
//        }
//        [HttpGet]
//        public IActionResult Users()
//        {
//            return View();
//        }
//        public IActionResult AddUser()
//        {
//            return PartialView();
//        }
//        [HttpPost]
//        public async Task<IActionResult>AddUser(AddUserRequest model)
//        {
//            try
//            {
//                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.RoleType))
//                {
//                    return Json(new { success = false, message = "Required Fields Missing" });
//                }
//                var user = new IdentityUser
//                {
//                    Email = model.Email,
//                    UserName = model.Email,
//                    PhoneNumber = model.PhoneNumber
//                };
//                var result = await _userManager.CreateAsync(user, model.Password);
//                if (!result.Succeeded)
//                {
//                    return Json(new { success = false, message = result.Errors.First().Description });
//                }
//                await _userManager.AddToRoleAsync(user, model.RoleType);
//                _subjectRepo.InsertUserProfile(user.Id, model);
//                return Json(new { success = true, message = "User Created Successfully" });
//            }
//            catch (Exception ex)
//            {
//                if (ex.Message.Contains("UQ_Student_Roll"))
//                    return Json(new { success = false, message = "Roll number already exists" });

//                if (ex.Message.Contains("UQ_Teacher_Subject"))
//                    return Json(new { success = false, message = "Subject already assigned to another teacher" });
//                if(ex.Message.Contains("UQ_User_Phone"))
//                    return Json(new { success = false, message = "Subject already assigned to another teacher" });
//                return Json(new { success = false, message = ex.Message });
//            }

//        }
//        [HttpGet]
//        public IActionResult LoadFreeSubjects(string Id)
//        {
//            var data = _subjectRepo.GetFreeSubjects(Id);
//            return Json(data);
//        }
//        [HttpGet]
//        public IActionResult EditSubject(int id)
//        {
//            var subject = _subjectRepo.GetSubjectById(id);
//            return PartialView(subject);
//        }
//        [HttpPost]
//        public IActionResult EditSubject(int id, string name)
//        {
//            try
//            {
//                _subjectRepo.UpdateSubject(id, name);
//                return Json(new { success = true, message = "Subject Updated" });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }
//        [HttpPost]
//        public IActionResult DeleteSubject(int id)
//        {
//            try
//            {
//                _subjectRepo.DeleteSubject(id);
//                return Json(new { success = true, message = "Subject Deleted" });
//            }
//            catch (Exception)
//            {
//                return Json(new { success = false, message = "Cannot delete subject. It is assigned to teacher." });
//            }
//        }
//        [HttpPost]
//        public IActionResult DeleteTeacher(string id)
//        {
//            try
//            {
//                if (string.IsNullOrEmpty(id))
//                    return Json(new { success = false, message = "Invalid id" });

//                _teacherRepo.DeleteTeacher(id);
//                return Json(new { success = true, message = "Teacher deleted" });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }
//        [HttpPost]
//        public IActionResult DeleteStudent(string id)
//        {
//            try
//            {
//                if (string.IsNullOrEmpty(id))
//                    return Json(new { success = false, message = "Invalid id" });

//                _studentRepo.DeleteStudent(id);
//                return Json(new { success = true, message = "Student deleted" });
//            }
//            catch(Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }
//        [HttpGet]
//        public IActionResult EditTeacher(string id)
//        {
//            var model = _teacherRepo.GetTeacherById(id);

//            ViewBag.Subjects = _subjectRepo.GetFreeSubjects(id);

//            return PartialView(model);
//        }

//        [HttpGet]
//        public IActionResult EditStudent(string id)
//        {
//            var data = _studentRepo.GetStudentByUserId(id);
//            return PartialView(data);
//        }
//        [HttpPost]
//        public IActionResult EditStudent(StudentListViewModel model)
//        {
//            try
//            {
//                _studentRepo.UpdateStudent(model);
//                return Json(new { success = true, message = "Student Updated" });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }
//        [HttpPost]
//        public IActionResult EditTeacher(TeacherEditViewModel model)
//        {
//            try
//            {
//                _teacherRepo.UpdateTeacher(model);
//                return Json(new { success = true, message = "Teacher Updated" });
//            }
//            catch (Exception ex)
//            {
//                if (ex.Message.Contains("UQ_Teacher_Subject"))
//                    return Json(new { success = false, message = "Subject already assigned" });

//                return Json(new { success = false, message = ex.Message });
//            }
//        }
//        [HttpPost]
//        public IActionResult LoadUsers()
//        {
//            try
//            {
//                var data = _userRepo.GetAllUsers();
//                return PartialView(data);
//            }
//            catch(Exception ex)
//            {
//                return Json(new {success=false, message=ex.Message});
//            }
//        }

//    }
//}

using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManegementNew.Models;
using SchoolManegementNew.Repositories;
using SchoolManegementNew.Services.Reports;
using System.IO;
namespace SchoolManegementNew.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // =========================
        // DEPENDENCIES
        // =========================

        private readonly IDashboardRepository _dashboardRepo;
        private readonly ITeacherRepository _teacherRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly ISubjectRepository _subjectRepo;
        private readonly IUserRepository _userRepo;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPdfReportService _pdfReportService;

        public AdminController(
            IDashboardRepository dashboardRepo,
            ITeacherRepository teacherRepo,
            IStudentRepository studentRepo,
            ISubjectRepository subjectRepo,
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            IUserRepository userRepo, IPdfReportService pdfReportService)
        {
            _dashboardRepo = dashboardRepo;
            _teacherRepo = teacherRepo;
            _studentRepo = studentRepo;
            _subjectRepo = subjectRepo;
            _roleManager = roleManager;
            _userManager = userManager;
            _userRepo = userRepo;
            _pdfReportService = pdfReportService;
        }

        //Export PDF
        public IActionResult ExportFullReportPdf()
        {
            byte[] pdfBytes = _pdfReportService.GenerateAdminFullReport();
            return File(pdfBytes, "application/pdf", "Admin_Full_Report.pdf");
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
        // TEACHERS
        // =========================

        public IActionResult Teachers()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoadTeachers()
        {
            try
            {
                var data = _teacherRepo.GetAllTeachers();
                return PartialView(data);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteTeacher(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return Json(new { success = false, message = "Invalid id" });

                _teacherRepo.DeleteTeacher(id);
                return Json(new { success = true, message = "Teacher deleted" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult EditTeacher(string id)
        {
            var model = _teacherRepo.GetTeacherById(id);

            // Used to show only free subjects in dropdown
            ViewBag.Subjects = _subjectRepo.GetFreeSubjects(id);

            return PartialView(model);
        }

        [HttpPost]
        public IActionResult EditTeacher(TeacherEditViewModel model)
        {
            try
            {
                _teacherRepo.UpdateTeacher(model);
                return Json(new { success = true, message = "Teacher Updated" });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UQ_Teacher_Subject"))
                    return Json(new { success = false, message = "Subject already assigned" });

                return Json(new { success = false, message = ex.Message });
            }
        }

        // =========================
        // STUDENTS
        // =========================

        public IActionResult Students()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoadStudents()
        {
            try
            {
                var data = _studentRepo.GetAllStudents();
                return PartialView(data);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteStudent(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return Json(new { success = false, message = "Invalid id" });

                _studentRepo.DeleteStudent(id);
                return Json(new { success = true, message = "Student deleted" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult EditStudent(string id)
        {
            var data = _studentRepo.GetStudentByUserId(id);
            return PartialView(data);
        }

        [HttpPost]
        public IActionResult EditStudent(StudentListViewModel model)
        {
            try
            {
                _studentRepo.UpdateStudent(model);
                return Json(new { success = true, message = "Student Updated" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =========================
        // SUBJECTS
        // =========================

        [HttpGet]
        public IActionResult Subjects()
        {
            return View();
        }

        [HttpGet]
        public IActionResult LoadSubjects()
        {
            try
            {
                var data = _subjectRepo.GetAllSubject();
                return PartialView("LoadSubjects", data);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult AddSubjects()
        {
            return PartialView("AddSubject");
        }

        [HttpPost]
        public IActionResult AddSubject(string subjectName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subjectName))
                    return Json(new { success = false, message = "Subject name required" });

                _subjectRepo.AddSubject(subjectName);
                return Json(new { success = true, message = "Subject Added Successfully" });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UQ_Subject_Name"))
                    return Json(new { success = false, message = "Subject Already Exist" });

                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult EditSubject(int id)
        {
            var subject = _subjectRepo.GetSubjectById(id);
            return PartialView(subject);
        }

        [HttpPost]
        public IActionResult EditSubject(int id, string name)
        {
            try
            {
                _subjectRepo.UpdateSubject(id, name);
                return Json(new { success = true, message = "Subject Updated" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteSubject(int id)
        {
            try
            {
                _subjectRepo.DeleteSubject(id);
                return Json(new { success = true, message = "Subject Deleted" });
            }
            catch
            {
                return Json(new { success = false, message = "Cannot delete subject. It is assigned to teacher." });
            }
        }

        [HttpGet]
        public IActionResult LoadFreeSubjects(string id)
        {
            var data = _subjectRepo.GetFreeSubjects(id);
            return Json(data);
        }

        // =========================
        // USERS
        // =========================

        [HttpGet]
        public IActionResult Users()
        {
            return View();
        }

        public IActionResult AddUser()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserRequest model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email) ||
                    string.IsNullOrEmpty(model.Password) ||
                    string.IsNullOrEmpty(model.RoleType))
                {
                    return Json(new { success = false, message = "Required Fields Missing" });
                }

                var user = new IdentityUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                    return Json(new { success = false, message = result.Errors.First().Description });

                await _userManager.AddToRoleAsync(user, model.RoleType);

                // Insert into UserProfiles table
                _subjectRepo.InsertUserProfile(user.Id, model);

                return Json(new { success = true, message = "User Created Successfully" });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UQ_Student_Roll"))
                    return Json(new { success = false, message = "Roll number already exists" });

                if (ex.Message.Contains("UQ_Teacher_Subject"))
                    return Json(new { success = false, message = "Subject already assigned to another teacher" });

                if (ex.Message.Contains("UQ_User_Phone"))
                    return Json(new { success = false, message = "Phone number already exists" });

                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult LoadUsers()
        {
            try
            {
                var data = _userRepo.GetAllUsers();
                return PartialView(data);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
