namespace Unison.LibraryManagement.Domain.Dto
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long Total { get; set; }
    }
}
