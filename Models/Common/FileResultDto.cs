namespace SchoolManegementNew.Models.Common
{
    public class FileResultDto
    {
        public byte[] Bytes { get; set; } = default!;
        public string ContentType { get; set; } = "application/pdf";
        public string FileName { get; set; } = "";
    }
}
