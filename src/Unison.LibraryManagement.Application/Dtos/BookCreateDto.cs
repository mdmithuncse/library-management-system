using System;

namespace Unison.LibraryManagement.Application.Dtos
{
    public class BookCreateDto
    {
        public string ISBN { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Authors { get; set; } = string.Empty;
        public int TotalCopies { get; set; }
    }
}
