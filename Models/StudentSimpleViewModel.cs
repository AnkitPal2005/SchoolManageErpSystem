namespace SchoolManegementNew.Models
{
    public class StudentSimpleViewModel
    {
        public string ?StudentUserId { get; set; }
        public string ?FullName { get; set; }
        public string ?RollNumber { get; set; }
        public int MarksObtained { get; set; }
        public int MaxMarks { get; set; }
        public string? SubjectName { get; set; }
        public string? MarksRowExists { get; set; } 
    }
}

