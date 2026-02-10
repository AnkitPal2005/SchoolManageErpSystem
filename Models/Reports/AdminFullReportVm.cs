using System;
using System.Collections.Generic;

namespace SchoolManegementNew.Models.Reports
{
    public class AdminFullReportVm
    {
        // SUMMARY SECTION
        public AdminReportSummary Summary { get; set; }

        // TABLE SECTIONS
        public List<TeacherReportDto> Teachers { get; set; }
        public List<StudentReportDto> Students { get; set; }
        public List<UserReportDto> Users { get; set; }

        // HEADER INFO (NECHO format – top right)
        public DateTime ReportDate { get; set; }
        public string ReportRef { get; set; }
        public string AcademicYear { get; set; }
     
    }
}
