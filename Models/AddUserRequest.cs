namespace SchoolManegementNew.Models
{
    public class AddUserRequest
    {
        public string? RoleType { get; set; }   
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }

        // Student Only
        public string? RollNumber { get; set; }

        // Teacher Only
        public int? SubjectId { get; set; }
    }
}
