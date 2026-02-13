//using iText.Kernel.Pdf;
//using iText.Layout;
//using iText.Layout.Element;
//using iText.Layout.Properties;
//using iText.StyledXmlParser.Css.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Playwright;
//using Org.BouncyCastle.Utilities;
using SchoolManegementNew.Models;
using SchoolManegementNew.Models.Reports;
using SchoolManegementNew.Repositories;
using SchoolManegementNew.Repositories.Reports;
using SchoolManegementNew.Services;
using SchoolManegementNew.Services.Reports;
using System.IO;
using System.Text;
using SchoolManegementNew.Services.Email;
using System.IO.Compression;

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
        private readonly IAdminReportRepository _reportRepo;
        private readonly IReportExportService _reportExportService;
        private readonly RazorViewRenderer _razorViewRenderer;
        private readonly EmailService _emailService;

        public AdminController(

            IDashboardRepository dashboardRepo,
            ITeacherRepository teacherRepo,
            IStudentRepository studentRepo,
            ISubjectRepository subjectRepo,
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            IUserRepository userRepo, IAdminReportRepository reportRepo, RazorViewRenderer razorViewRenderer, IReportExportService reportExportService, EmailService emailService)
        {
            _dashboardRepo = dashboardRepo;
            _teacherRepo = teacherRepo;
            _studentRepo = studentRepo;
            _subjectRepo = subjectRepo;
            _roleManager = roleManager;
            _userManager = userManager;
            _userRepo = userRepo;
            _reportRepo = reportRepo;
            _reportExportService = reportExportService;
            _razorViewRenderer = razorViewRenderer;
            _emailService = emailService;
        }
        [HttpPost]
        public async Task<IActionResult> SendUsersZipByEmail(
    string toEmail,
    string? search,
    string? role,
    int? subjectId
)
        {
            
            var result = await _userRepo.GetUserPagedAsync(
                pageNumber: 1,
                pageSize: int.MaxValue,
                search: search,
                role: role,
                subjectId: subjectId
            );

           
            string pdfHtml = _razorViewRenderer.RenderToString(
                ControllerContext,
                "UsersPdf",
                result.Items
            );

            byte[] pdfBytes;
            using (var playwright = await Playwright.CreateAsync())
            await using (var browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions { Headless = true }))
            {
                var page = await browser.NewPageAsync();
                await page.SetContentAsync(pdfHtml);
                pdfBytes = await page.PdfAsync(new PagePdfOptions
                {
                    Format = "A4",
                    PrintBackground = true
                });
            }

            
            byte[] excelBytes =
                _reportExportService.GenerateFilteredUsersExcel(result.Items.ToList());

            
            var sb = new StringBuilder();
            sb.AppendLine("Name,Email,Phone,Role,Subject/Roll");

            foreach (var u in result.Items)
            {
                var subjectOrRoll = u.UserType == "Teacher" ? u.SubjectName : u.RollNumber;
                sb.AppendLine(
                    $"{Escape(u.FullName)}," +
                    $"{Escape(u.Email)}," +
                    $"{Escape(u.PhoneNumber)}," +
                    $"{Escape(u.UserType)}," +
                    $"{Escape(subjectOrRoll)}"
                );
            }

            byte[] csvBytes = Encoding.UTF8.GetBytes(sb.ToString());

            byte[] zipBytes;

            using (var zipStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                {
                    var pdfEntry = archive.CreateEntry("FilteredUsers.pdf");
                    using (var entry = pdfEntry.Open())
                        await entry.WriteAsync(pdfBytes);

                    var excelEntry = archive.CreateEntry("FilteredUsers.xlsx");
                    using (var entry = excelEntry.Open())
                        await entry.WriteAsync(excelBytes);

                    var csvEntry = archive.CreateEntry("FilteredUsers.csv");
                    using (var entry = csvEntry.Open())
                        await entry.WriteAsync(csvBytes);
                }

                zipBytes = zipStream.ToArray();
            }

           
            string emailBody = @"
    <h2>School Management System</h2>
    <p>Please find attached the <strong>Filtered Users Report</strong> ZIP file.</p>
    <p>This ZIP contains PDF, Excel and CSV reports.</p>
    <br/>
    <p>Regards,<br/><strong>School Management System</strong></p>
    ";

            // ======================
            // 7. SEND EMAIL WITH ZIP
            // ======================
            await _emailService.SendEmailAsync(
                toEmail,
                "Filtered Users Report (ZIP)",
                emailBody,
                zipBytes,
                "FilteredUsersReport.zip"
            );

            return Json(new
            {
                success = true,
                message = "ZIP report emailed successfully"
            });
        }

        //Export PDF


        public async Task<IActionResult> ExportFullReportPdf()
        {
            var model = new AdminFullReportVm
            {
                Summary = _reportRepo.GetSummary(),
                Teachers = _reportRepo.GetTeachers(),
                Students = _reportRepo.GetStudents(),
                Users = _reportRepo.GetUsers(),

                ReportDate = DateTime.Now,
                AcademicYear = "2025-26",
                ReportRef = "SCH-RPT-" + DateTime.Now.ToString("yyyyMMdd")
            };

            string html = _razorViewRenderer.RenderToString(
                ControllerContext,
                "SchoolReport",
                model
            );

            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions { Headless = true });

            var page = await browser.NewPageAsync();
            await page.SetContentAsync(html);

            byte[] pdf = await page.PdfAsync(new PagePdfOptions
            {
                Format = "A4",
                PrintBackground = true
            });

            return File(pdf, "application/pdf", "SchoolReport.pdf");
        }
        [HttpGet]
        public async Task<IActionResult> ExportUsersPdf(string? search, string? role, int? subjectId)
        {
            var result = await _userRepo.GetUserPagedAsync(
                pageNumber: 1,
                pageSize: int.MaxValue,
                search: search,
                role: role,
                subjectId: subjectId
                );
            string html = _razorViewRenderer.RenderToString(
                ControllerContext,
                 "UsersPdf",
                result.Items
                );
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions { Headless = true }
                );
            var page = await browser.NewPageAsync();
            await page.SetContentAsync(html);
            byte[] pdf = await page.PdfAsync(
                new PagePdfOptions
                {
                    Format = "A4",
                    PrintBackground = true
                });
            return File(pdf, "application/pdf", "FilteredUser.pdf");
        }
        //Send Pdf To mail
        [HttpPost]
        public async Task<IActionResult> SendUsersPdfByEmail(string toEmail, string? search, string? role, int? subjectId)
        {
            var result = await _userRepo.GetUserPagedAsync(pageNumber: 1, pageSize: int.MaxValue, search: search, role: role, subjectId: subjectId);
            string html = _razorViewRenderer.RenderToString(
                ControllerContext,
                "UsersPdf",
                result.Items
                );
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions { Headless = true }
                );
            var page = await browser.NewPageAsync();
            await page.SetContentAsync(html);
            byte[] pdfbytes = await page.PdfAsync(new PagePdfOptions
            {
                Format = "A4",
                PrintBackground = true
            });
            string emailBody = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
</head>
<body style='margin:0;padding:0;background-color:#f4f6f8;font-family:Arial,Helvetica,sans-serif;'>

<table width='100%' cellpadding='0' cellspacing='0'>
<tr>
<td align='center' style='padding:30px 0;'>

<table width='600' cellpadding='0' cellspacing='0'
style='background:#ffffff;border-radius:8px;box-shadow:0 4px 12px rgba(0,0,0,0.1);overflow:hidden;'>

<tr>
<td style='background:#0d6efd;color:#ffffff;padding:20px;text-align:center;'>

<img src=""https://i.pinimg.com/736x/f8/a3/6d/f8a36de29089f1e66c503638c4ae0337.jpg""
     alt=""School Logo""
     style=""max-height:80px;margin-bottom:10px;"" />

<h2 style='margin:0;'>School Management System</h2>
<p style='margin:5px 0 0;font-size:14px;'>Filtered User Report</p>
</td>
</tr>

<tr>
<td style='padding:25px;color:#333333;'>
<p style='font-size:15px;'>Hello <strong>Ankit</strong>,</p>

<p style='font-size:14px;line-height:1.6;'>
Please find attached the <strong>Filtered Users Report</strong> you requested.
</p>

<div style='margin:20px 0;padding:15px;background:#f8f9fa;border-left:4px solid #0d6efd;'>
<p style='margin:0;font-size:14px;'>
📄 <strong>Attachment:</strong> FilteredUser.pdf
</p>
</div>

<p style='font-size:14px;'>
If you need any changes, feel free to reply to this email.
</p>

<p style='margin-top:30px;font-size:14px;'>
Regards,<br>
<strong>School Management System</strong>
</p>
</td>
</tr>

<tr>
<td style='background:#f1f1f1;padding:15px;text-align:center;font-size:12px;color:#666666;'>
© 2026 School Management System<br>
This is an auto-generated email.
</td>
</tr>

</table>

</td>
</tr>
</table>

</body>
</html>";

            await _emailService.SendEmailAsync(
                toEmail,
                "Filtered User Report",
               emailBody,
                pdfbytes,
                "FilteredUser.pdf"
                );
            return Json(new { success = true, message = "Email sent successfully" });
        }
        public IActionResult ExportFullReportExcel()
        {
            var fileBytes = _reportExportService.GenerateExcelReport();

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "SchoolManagementReport.xlsx"
            );
        }
        public async Task<IActionResult> ExportUsersExcel(string? search, string? role, int? subjectId)
        {
            var result = await _userRepo.GetUserPagedAsync(
                pageNumber: 1,
                pageSize: int.MaxValue,
                search: search,
                role: role,
                subjectId: subjectId
                );
            var excelBytes = _reportExportService.GenerateFilteredUsersExcel(result.Items.ToList());
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "FilteredUsers.xlsx");
        }
        [HttpGet]
        public async Task<IActionResult> ExportUsersCsv(string? search, string? role, int? subjectId)
        {
            var result = await _userRepo.GetUserPagedAsync(
                pageNumber: 1,
                pageSize: int.MaxValue,
                search: search,
                role: role,
                subjectId: subjectId
                );
            var users = result.Items;
            var sb = new StringBuilder();
            sb.AppendLine("Name,Email,Phone,Role,Subject/Roll");
            foreach (var u in users)
            {
                var subjectOrRoll = u.UserType == "Teacher" ? u.SubjectName : u.RollNumber;
                sb.AppendLine(
                    $"{Escape(u.FullName)}," +
                    $"{Escape(u.Email)}," +
                    $"{Escape(u.PhoneNumber)}," +
                    $"{Escape(u.UserType)}," +
                    $"{Escape(subjectOrRoll)}"
                    );
            }


            var bytes = Encoding.UTF8.GetBytes(sb.ToString());

            return File(
        bytes,
       "text/csv",
        "FilteredUsers.csv"
    );

        }
        private string Escape(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            if (value.Contains(",") || value.Contains("\""))
                return $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
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


        [HttpGet]
        public async Task<IActionResult> LoadTeachers(string? search, int page = 1, int pageSize = 5)
        {
            var result = await _teacherRepo.GetTeachersPagedAsync(page, pageSize, search);

            return PartialView("LoadTeachers", result);
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
                if (model.SubjectId == 0 || model.SubjectId == null)
                {
                    model.SubjectId = null;
                }

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

        [HttpGet]
        public async Task<IActionResult> LoadStudentsBySearch(string? search, int page = 1, int pageSize = 5)
        {


            var result = await _reportRepo.GetStudentsPagedAsync(page, pageSize, search);

            return PartialView("LoadStudents", result);
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
        public async Task<IActionResult> LoadSubjects(string? search, int page = 1, int pageSize = 5)
        {
            var result = await _subjectRepo.GetSubjectsPagedAsync(page, pageSize, search);

            return PartialView("LoadSubjects", result);
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
        public async Task<IActionResult> Users()
        {

            var roles = await _userRepo.GetAllRolesAsync();
            var subjects = await _userRepo.GetAllSubjectsAsync();
            ViewBag.Subjects = subjects;
            ViewBag.Roles = roles;
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
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed=true
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

        [HttpGet]
        public async Task<IActionResult> LoadUsers(string? search, string? role = null, int page = 1, int pageSize = 5, int? subjectId = null)
        {
            try
            {
                var data = await _userRepo.GetUserPagedAsync(page, pageSize, search, role, subjectId);
                return PartialView(data);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}

