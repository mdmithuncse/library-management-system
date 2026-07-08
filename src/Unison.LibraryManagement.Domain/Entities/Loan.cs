using System;

namespace Unison.LibraryManagement.Domain.Entities
{
    public class Loan
    {
        public Guid Id { get; set; }
        public Guid BookCopyId { get; set; }
        public Guid UserId { get; set; }
        public DateTime BorrowedAt { get; set; }
        public DateTime DueAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public decimal? FineAmountPaid { get; set; }

        // Navigation
        public BookCopy? BookCopy { get; set; }
    }
}
