using System;

namespace Unison.LibraryManagement.Application.Commands
{
    public class BorrowBookCommand
    {
        public Guid UserId { get; set; }
        public Guid? BookId { get; set; }
        public string? ISBN { get; set; }
    }
}
