//using iText.IO.Image;
//using iText.Kernel.Colors;
//using iText.Kernel.Pdf;
//using iText.Kernel.Pdf.Canvas;
//using iText.Kernel.Pdf.Event;
//using iText.Layout;
////using System.IO;
//using iText.Layout.Borders;
//using iText.Layout.Element;
//using iText.Layout.Properties;
//using SchoolManegementNew.Models.Reports;
//using SchoolManegementNew.Repositories.Reports;
//using iText.Kernel.Geom;
//using System.IO;

//namespace SchoolManegementNew.Services.Reports
//{
//    public class PdfReportService : IPdfReportService
//    {
//        private readonly IAdminReportRepository _reportRepo;

//        public PdfReportService(IAdminReportRepository reportRepo)
//        {
//            _reportRepo = reportRepo;
//        }



//        public byte[] GenerateAdminFullReport()
//        {

//            //  1. DATA COLLECT (Repository se)
//            AdminReportSummary summary = _reportRepo.GetSummary();
//            List<TeacherReportDto> teachers = _reportRepo.GetTeachers();
//            List<StudentReportDto> students = _reportRepo.GetStudents();
//            List<UserReportDto> users = _reportRepo.GetUsers();

//            //  2. PDF MEMORY SETUP
//            using var stream = new MemoryStream();
//            PdfWriter writer = new PdfWriter(stream);
//            PdfDocument pdf = new PdfDocument(writer);
//            Document document = new Document(pdf);

//            //  3. HEADER
//            AddHeader(document);

//            //  4. SUMMARY
//            AddSummarySection(document, summary);

//            //  5. TEACHERS
//            AddTeachersSection(document, teachers);

//            //  6. STUDENTS
//            AddStudentsSection(document, students);

//            //  7. USERS
//            AddUsersSection(document, users);

//            document.Close();
//            return stream.ToArray();
//        }

//        // ================== PRIVATE HELPERS ==================

//        private void AddHeader(Document document)
//        {
//            Table headerTable = new Table(new float[] { 20,67,30 });
//            headerTable.AddCell(
//    new Cell()
//        .SetBackgroundColor(new DeviceRgb(201, 162, 39)) // gold
//        .SetBorder(Border.NO_BORDER)
//);

//            headerTable.UseAllAvailableWidth();
//            Table leftTable = new Table(1);
//            leftTable.SetBorder(Border.NO_BORDER);
//            string logoPath = System.IO.Path.Combine(
//                Directory.GetCurrentDirectory(),
//                "wwwroot",
//                "images",
//                "school-logo.png.jpg"
//                );
//            if (System.IO.File.Exists(logoPath))
//            {
//                Image logo = new Image(ImageDataFactory.Create(logoPath))
//                    .SetWidth(100);
//                leftTable.AddCell(
//                    new Cell()
//                    .SetBorder(Border.NO_BORDER)
//                    .Add(logo)
//                    );
//            }
//            leftTable.AddCell(NoBorderCell("Necho Public School", 12, true));
//            leftTable.AddCell(NoBorderCell(@"
//Plot No. D-258, Phase 8-A, Industrial Area, Sector 75
//Sohana, Rupnagar, S.A.S. Nagar (Mohali)
//Punjab, India, 140308
//+91-7717474088 | info@nechonetworks.com | www.nechonetworks.com"));
//            leftTable.AddCell(NoBorderCell(@"Sohana, Rupnagar, S.A.S. Nagar (Mohali)
//Punjab, India, 140308"));
//            leftTable.AddCell(NoBorderCell("Phone: 9876543210 | Email: school@email.com"));
//            headerTable.AddCell(
//                new Cell()
//                .SetBorder(Border.NO_BORDER)
//                .Add(leftTable)
//                );
//            Table rightTable = new Table(1);
//            rightTable.SetBorder(Border.NO_BORDER);

//            rightTable.AddCell(NoBorderCell($"Date: {DateTime.Now:dd-MM-yyyy}", 9, true));
//            rightTable.AddCell(NoBorderCell("Admin Report", 9));
//            rightTable.AddCell(NoBorderCell("Academic Year: 2025-26", 9));
//            headerTable.AddCell(
//               new Cell()
//                   .SetBorder(Border.NO_BORDER)
//                   .SetTextAlignment(TextAlignment.RIGHT)
//                   .Add(rightTable)
//           );
//            document.Add(headerTable);
//            document.Add(
//                new Paragraph("")
//                    .SetBorderBottom(
//                        new SolidBorder(new DeviceRgb(201, 162, 39), 2)
//                    )
//                    .SetMarginTop(10)
//                    .SetMarginBottom(20)
//            );


//            document.Add(
//                new Paragraph("School Management System Report")
//                    .SetFontSize(16)
//                    //.SetBold()
//                    .SetMarginBottom(20)
//            );
//        }

//        private void AddSummarySection(Document document, AdminReportSummary summary)
//        {

//            document.Add(
//                new Paragraph(new Text("SUMMARY"))
//                    .SetFontSize(14)
//                    .SetMarginBottom(10)
//            );

//            Table table = new Table(2).UseAllAvailableWidth();

//            table.AddCell("Total Teachers");
//            table.AddCell(summary.TotalTeachers.ToString());

//            table.AddCell("Total Students");
//            table.AddCell(summary.TotalStudents.ToString());

//            table.AddCell("Total Subjects");
//            table.AddCell(summary.TotalSubjects.ToString());

//            table.AddCell("Total Users");
//            table.AddCell(summary.TotalUsers.ToString());

//            document.Add(table);
//        }

//        private void AddTeachersSection(Document document, List<TeacherReportDto> teachers)
//        {
//            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

//            document.Add(
//                new Paragraph(new Text("TEACHERS INFORMATION"))
//                    .SetFontSize(14)
//                    .SetMarginBottom(10)
//            );

//            Table table = new Table(4).UseAllAvailableWidth();

//            AddHeaderCell(table, "Name");
//            AddHeaderCell(table, "Email");
//            AddHeaderCell(table, "Subject");
//            AddHeaderCell(table, "Status");

//            foreach (var t in teachers)
//            {
//                table.AddCell(t.Name);
//                table.AddCell(t.Email);
//                table.AddCell(t.SubjectName);
//                table.AddCell(t.Status);
//            }

//            document.Add(table);
//        }

//        private void AddStudentsSection(Document document, List<StudentReportDto> students)
//        {
//            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

//            document.Add(
//                new Paragraph(new Text("STUDENTS INFORMATION"))
//                    .SetFontSize(14)
//                    .SetMarginBottom(10)
//            );

//            Table table = new Table(4).UseAllAvailableWidth();

//            AddHeaderCell(table, "Roll No");
//            AddHeaderCell(table, "Name");
//            AddHeaderCell(table, "Email");
//            AddHeaderCell(table, "Total Subjects");

//            foreach (var s in students)
//            {
//                table.AddCell(s.RollNumber.ToString());
//                table.AddCell(s.Name);
//                table.AddCell(s.Email);
//                table.AddCell(s.TotalSubjects.ToString());
//            }

//            document.Add(table);
//        }

//        private void AddUsersSection(Document document, List<UserReportDto> users)
//        {
//            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

//            document.Add(
//                new Paragraph(new Text("ALL USERS"))
//                    .SetFontSize(14)
//                    .SetMarginBottom(10)
//            );

//            Table table = new Table(3).UseAllAvailableWidth();

//            AddHeaderCell(table, "Name");
//            AddHeaderCell(table, "Email");
//            AddHeaderCell(table, "Role");

//            foreach (var u in users)
//            {
//                table.AddCell(u.Name);
//                table.AddCell(u.Email);
//                table.AddCell(u.Role);
//            }

//            document.Add(table);
//        }

//        private void AddHeaderCell(Table table, string text)
//        {
//            table.AddHeaderCell(
//                new Cell().Add(new Paragraph(new Text(text)))
//            );
//        }

//        private Cell NoBorderCell(string text, int fontSize = 10, bool bold = false)
//        {
//            Paragraph p = new Paragraph(text).SetFontSize(fontSize);
//            //if (bold) p.SetBold();

//            return new Cell()
//                .SetBorder(Border.NO_BORDER)
//                .Add(p);
//        }
//    }
//}
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using SchoolManegementNew.Models.Reports;
using SchoolManegementNew.Repositories.Reports;
using iText.Kernel.Geom;
using System.IO;

namespace SchoolManegementNew.Services.Reports
{
    public class PdfReportService : IPdfReportService
    {
        private readonly IAdminReportRepository _reportRepo;

        // Define colors for consistent branding
        private readonly DeviceRgb GOLD_COLOR = new DeviceRgb(201, 162, 39);
        private readonly DeviceRgb HEADER_BG = new DeviceRgb(201, 162, 39);
        private readonly DeviceRgb TABLE_HEADER_BG = new DeviceRgb(218, 186, 85);
        private readonly DeviceRgb BORDER_COLOR = new DeviceRgb(200, 200, 200);

        public PdfReportService(IAdminReportRepository reportRepo)
        {
            _reportRepo = reportRepo;
        }

        public byte[] GenerateAdminFullReport()
        {
            // 1. DATA COLLECT (Repository se)
            AdminReportSummary summary = _reportRepo.GetSummary();
            List<TeacherReportDto> teachers = _reportRepo.GetTeachers();
            List<StudentReportDto> students = _reportRepo.GetStudents();
            List<UserReportDto> users = _reportRepo.GetUsers();

            // 2. PDF MEMORY SETUP
            using var stream = new MemoryStream();
            PdfWriter writer = new PdfWriter(stream);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf, PageSize.A4);

            // Set margins
            document.SetMargins(40, 40, 40, 40);

            // 3. HEADER
            AddHeader(document);

            // 4. SUMMARY SECTION
            AddSummarySection(document, summary);

            // 5. TEACHERS SECTION
            AddTeachersSection(document, teachers);

            // 6. STUDENTS SECTION
            AddStudentsSection(document, students);

            // 7. USERS SECTION
            AddUsersSection(document, users);

            document.Close();
            return stream.ToArray();
        }

        // ================== PRIVATE HELPERS ==================

        private void AddHeader(Document document)
        {
            // Create main header table with gold left border
            Table mainTable = new Table(UnitValue.CreatePercentArray(new float[] { 2, 98 }));
            mainTable.UseAllAvailableWidth();

            // Gold left border
            mainTable.AddCell(new Cell()
                .SetBackgroundColor(GOLD_COLOR)
                .SetBorder(Border.NO_BORDER)
                .SetHeight(150));

            // Content table
            Table contentTable = new Table(UnitValue.CreatePercentArray(new float[] { 60, 40 }));
            contentTable.UseAllAvailableWidth();
            contentTable.SetBorder(Border.NO_BORDER);

            // LEFT SIDE - Logo and School Info
            Table leftTable = new Table(1);
            leftTable.UseAllAvailableWidth();
            leftTable.SetBorder(Border.NO_BORDER);

            // Logo
            string logoPath = System.IO.Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "images",
                "school-logo.png.jpg"  // Make sure this is correct
            );

            if (File.Exists(logoPath))
            {
                try
                {
                    Image logo = new Image(ImageDataFactory.Create(logoPath))
                        .SetWidth(80)
                        .SetHeight(50)
                        .SetMarginBottom(10);

                    leftTable.AddCell(new Cell()
                        .SetBorder(Border.NO_BORDER)
                        .Add(logo)
                        .SetPaddingBottom(5));
                }
                catch { }
            }

            // School Name - Bold and larger
            leftTable.AddCell(CreateStyledCell("NECHO NETWORKS PRIVATE LIMITED",
                11, true, Border.NO_BORDER).SetPaddingBottom(3));

            // Address lines
            leftTable.AddCell(CreateStyledCell(
                "Plot No. D-258, Phase 8-A, Industrial Area, Sector 75",
                8, false, Border.NO_BORDER).SetPaddingTop(0).SetPaddingBottom(0));

            leftTable.AddCell(CreateStyledCell(
                "Sohana, Rupnagar, S.A.S. Nagar (Mohali)",
                8, false, Border.NO_BORDER).SetPaddingTop(0).SetPaddingBottom(0));

            leftTable.AddCell(CreateStyledCell(
                "Punjab, India, 140308",
                8, false, Border.NO_BORDER).SetPaddingTop(0).SetPaddingBottom(3));

            leftTable.AddCell(CreateStyledCell(
                "+91-7717474088 | info@nechonetworks.com | www.nechonetworks.com",
                8, false, Border.NO_BORDER).SetPaddingTop(0));

            // RIGHT SIDE - Document Info
            Table rightTable = new Table(1);
            rightTable.UseAllAvailableWidth();
            rightTable.SetBorder(Border.NO_BORDER);
            rightTable.SetTextAlignment(TextAlignment.RIGHT);

            rightTable.AddCell(CreateStyledCell($"Date: {DateTime.Now:dd-MM-yyyy}",
                9, true, Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT));

            rightTable.AddCell(CreateStyledCell($"Report Ref: SCH-RPT-{DateTime.Now:yyyyMMdd}",
                9, false, Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT));

            rightTable.AddCell(CreateStyledCell($"Academic Year: 2025-26",
                9, false, Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT));

            // Add left and right to content table
            contentTable.AddCell(new Cell()
                .SetBorder(Border.NO_BORDER)
                .Add(leftTable)
                .SetPadding(10));

            contentTable.AddCell(new Cell()
                .SetBorder(Border.NO_BORDER)
                .Add(rightTable)
                .SetPadding(10));

            // Add content to main table
            mainTable.AddCell(new Cell()
                .SetBorder(Border.NO_BORDER)
                .Add(contentTable));

            document.Add(mainTable);

            // Gold separator line
            document.Add(new Paragraph("")
                .SetBorderBottom(new SolidBorder(GOLD_COLOR, 2))
                .SetMarginTop(5)
                .SetMarginBottom(15));

            // Report Title
            document.Add(new Paragraph("School Management System Report")
                .SetFontSize(16)
                //.SetBold()
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(20));
        }

        private void AddSummarySection(Document document, AdminReportSummary summary)
        {
            // Section Title with gold background
            Table sectionTitle = new Table(1).UseAllAvailableWidth();
            sectionTitle.AddCell(new Cell()
                .Add(new Paragraph("SUMMARY")
                    .SetFontSize(11)
                    //.SetBold()
                    .SetFontColor(ColorConstants.WHITE))
                .SetBackgroundColor(HEADER_BG)
                .SetBorder(Border.NO_BORDER)
                .SetPadding(8)
                .SetMarginBottom(10));

            document.Add(sectionTitle);

            // Summary table - 2 columns
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 }));
            table.UseAllAvailableWidth();

            // Add summary data with borders
            AddDataRow(table, "Total Teachers:", summary.TotalTeachers.ToString());
            AddDataRow(table, "Total Students:", summary.TotalStudents.ToString());
            AddDataRow(table, "Total Subjects:", summary.TotalSubjects.ToString());
            AddDataRow(table, "Total Users:", summary.TotalUsers.ToString());

            document.Add(table);
            document.Add(new Paragraph("").SetMarginBottom(20)); // Spacing
        }

        private void AddTeachersSection(Document document, List<TeacherReportDto> teachers)
        {
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

            // Section Title
            AddSectionTitle(document, "TEACHERS INFORMATION");

            // Table with 4 columns
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 30, 30, 25, 15 }));
            table.UseAllAvailableWidth();

            // Header row
            AddTableHeaderCell(table, "Name");
            AddTableHeaderCell(table, "Email");
            AddTableHeaderCell(table, "Subject");
            AddTableHeaderCell(table, "Status");

            // Data rows
            bool alternate = false;
            foreach (var t in teachers)
            {
                AddTableDataCell(table, t.Name, alternate);
                AddTableDataCell(table, t.Email, alternate);
                AddTableDataCell(table, t.SubjectName, alternate);
                AddTableDataCell(table, t.Status, alternate);
                alternate = !alternate;
            }

            document.Add(table);
        }

        private void AddStudentsSection(Document document, List<StudentReportDto> students)
        {
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

            // Section Title
            AddSectionTitle(document, "STUDENTS INFORMATION");

            // Table with 4 columns
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 20, 35, 30, 15 }));
            table.UseAllAvailableWidth();

            // Header row
            AddTableHeaderCell(table, "Roll No");
            AddTableHeaderCell(table, "Name");
            AddTableHeaderCell(table, "Email");
            AddTableHeaderCell(table, "Total Subjects");

            // Data rows
            bool alternate = false;
            foreach (var s in students)
            {
                AddTableDataCell(table, s.RollNumber.ToString(), alternate);
                AddTableDataCell(table, s.Name, alternate);
                AddTableDataCell(table, s.Email, alternate);
                AddTableDataCell(table, s.TotalSubjects.ToString(), alternate);
                alternate = !alternate;
            }

            document.Add(table);
        }

        private void AddUsersSection(Document document, List<UserReportDto> users)
        {
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

            // Section Title
            AddSectionTitle(document, "ALL USERS");

            // Table with 3 columns
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 35, 40, 25 }));
            table.UseAllAvailableWidth();

            // Header row
            AddTableHeaderCell(table, "Name");
            AddTableHeaderCell(table, "Email");
            AddTableHeaderCell(table, "Role");

            // Data rows
            bool alternate = false;
            foreach (var u in users)
            {
                AddTableDataCell(table, u.Name, alternate);
                AddTableDataCell(table, u.Email, alternate);
                AddTableDataCell(table, u.Role, alternate);
                alternate = !alternate;
            }

            document.Add(table);
        }

        // ================== UTILITY METHODS ==================

        private void AddSectionTitle(Document document, string title)
        {
            Table sectionTitle = new Table(1).UseAllAvailableWidth();
            sectionTitle.AddCell(new Cell()
                .Add(new Paragraph(title)
                    .SetFontSize(11)
                    //.SetBold()
                    .SetFontColor(ColorConstants.WHITE))
                .SetBackgroundColor(HEADER_BG)
                .SetBorder(Border.NO_BORDER)
                .SetPadding(8)
                .SetMarginBottom(10));

            document.Add(sectionTitle);
        }

        private void AddTableHeaderCell(Table table, string text)
        {
            table.AddHeaderCell(new Cell()
                .Add(new Paragraph(text).SetFontSize(9))
                .SetBackgroundColor(TABLE_HEADER_BG)
                .SetBorder(new SolidBorder(BORDER_COLOR, 1))
                .SetPadding(6)
                .SetTextAlignment(TextAlignment.LEFT));
        }

        private void AddTableDataCell(Table table, string text, bool alternate)
        {
            Cell cell = new Cell()
                .Add(new Paragraph(text).SetFontSize(9))
                .SetBorder(new SolidBorder(BORDER_COLOR, 0.5f))
                .SetPadding(6)
                .SetTextAlignment(TextAlignment.LEFT);

            if (alternate)
            {
                cell.SetBackgroundColor(new DeviceRgb(250, 250, 250));
            }

            table.AddCell(cell);
        }

        private void AddDataRow(Table table, string label, string value)
        {
            // Label cell
            table.AddCell(new Cell()
                .Add(new Paragraph(label).SetFontSize(9))
                .SetBorder(new SolidBorder(BORDER_COLOR, 1))
                .SetBackgroundColor(new DeviceRgb(245, 245, 245))
                .SetPadding(6));

            // Value cell
            table.AddCell(new Cell()
                .Add(new Paragraph(value).SetFontSize(9))
                .SetBorder(new SolidBorder(BORDER_COLOR, 1))
                .SetPadding(6));
        }

        private Cell CreateStyledCell(string text, int fontSize, bool bold, Border border)
        {
            Paragraph p = new Paragraph(text).SetFontSize(fontSize);
            //if (bold)
            //{
            //    p.SetBold();
            //}

            return new Cell()
                .SetBorder(border)
                .Add(p);
        }
    }
}