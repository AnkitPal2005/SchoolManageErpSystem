using ClosedXML.Excel;
using SchoolManegementNew.Repositories.Reports;
namespace SchoolManegementNew.Services.Reports
{
    public class ReportExportService: IReportExportService
    {
        private readonly IAdminReportRepository _reportRepo;
        public ReportExportService(IAdminReportRepository reportRepo) { 
        _reportRepo = reportRepo;
        }
        public byte[] GenerateExcelReport()
        {
            var summary = _reportRepo.GetSummary();
            var teachers = _reportRepo.GetTeachers();
            var students= _reportRepo.GetStudents();
            var users= _reportRepo.GetUsers();
            var marks = _reportRepo.GetStudentMarks();
            using var workbook = new XLWorkbook();
            //Sheet 1 Summary
            var wsSummary=workbook.Worksheets.Add("Summary");
            wsSummary.Cell(1, 1).Value = "Data";
            wsSummary.Cell(1, 2).Value = "Information";
            wsSummary.Cell(2, 1).Value = "Total Teachers";
            wsSummary.Cell(2, 2).Value = summary.TotalTeachers;
            wsSummary.Cell(3, 1).Value = "Total Students";
            wsSummary.Cell(3, 2).Value = summary.TotalStudents;
            wsSummary.Cell(4, 1).Value = "Total Subjects";
            wsSummary.Cell(4, 2).Value = summary.TotalSubjects;
            wsSummary.Cell(5, 1).Value = "Total Users";
            wsSummary.Cell(5, 2).Value = summary.TotalUsers;
            wsSummary.Columns().AdjustToContents();

            //Sheet 2 Teachers
            var wsTeachers = workbook.Worksheets.Add("Teachers");
            wsTeachers.Cell(1, 1).InsertTable(teachers);
            wsTeachers.Columns().AdjustToContents();

            //Sheet 3 Student
            var wsStudents = workbook.Worksheets.Add("Students");

            wsStudents.Cell(1, 1).Value = "Roll No";
            wsStudents.Cell(1, 2).Value = "Student Name";
            wsStudents.Cell(1, 3).Value = "Subject";
            wsStudents.Cell(1, 4).Value = "Marks Obtained";
            wsStudents.Cell(1, 5).Value = "Max Marks";

            int row = 2;

            foreach (var m in marks)
            {
                wsStudents.Cell(row, 1).Value = m.RollNumber;
                wsStudents.Cell(row, 2).Value = m.StudentName;
                wsStudents.Cell(row, 3).Value = m.SubjectName;
                wsStudents.Cell(row, 4).Value = m.MarksObtained;
                wsStudents.Cell(row, 5).Value = m.MaxMarks;
                row++;
            }

            wsStudents.Columns().AdjustToContents();

            //Sheet 4 Subjects
            var wsUsers = workbook.Worksheets.Add("Users");
            wsUsers.Cell(1, 1).InsertTable(users);
            wsUsers.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
