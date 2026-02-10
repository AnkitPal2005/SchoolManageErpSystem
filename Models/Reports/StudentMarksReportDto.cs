namespace SchoolManegementNew.Models.Reports
{
    public class StudentMarksReportDto
    {
        public string RollNumber { get; set; }
        public string StudentName { get; set; }
        public string SubjectName { get; set; }
        public int MarksObtained { get; set; }
        public int MaxMarks { get; set; }
    }
}
