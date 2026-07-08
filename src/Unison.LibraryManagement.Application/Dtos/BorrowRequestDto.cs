using System;

namespace Unison.LibraryManagement.Application.Dtos
{
    public class BorrowRequestDto
    {
        public Guid BookId { get; set; }
        // Optionally allow ISBN
        public string? ISBN { get; set; }
    }
}
