namespace SchoolManegementNew.Models
{
    public class PaginationViewModel<T>
    {
        public IEnumerable<T>Items { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
