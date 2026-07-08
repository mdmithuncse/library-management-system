namespace Unison.LibraryManagement.Domain.Entities
{
    public class Book
    {
        public Guid Id { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Authors { get; set; } = string.Empty;
        public string? Publisher { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string? Description { get; set; }
        public int TotalCopies { get; set; }

        // Navigation
        public List<BookCopy> Copies { get; set; } = new();
    }
}
