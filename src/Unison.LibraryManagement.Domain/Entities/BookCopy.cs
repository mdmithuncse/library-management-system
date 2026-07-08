namespace Unison.LibraryManagement.Domain.Entities
{
    public class BookCopy
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public string CopyNumber { get; set; } = string.Empty;
        public BookCopyStatus Status { get; set; } = BookCopyStatus.Available;

        // Navigation
        public Book? Book { get; set; }
    }
}
