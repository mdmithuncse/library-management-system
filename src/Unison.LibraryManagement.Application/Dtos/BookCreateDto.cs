using System;
using System.ComponentModel.DataAnnotations;

namespace Unison.LibraryManagement.Application.Dtos
{
    public class BookCreateDto
    {
        [Required]
        [StringLength(32)]
        public string ISBN { get; set; } = string.Empty;

        [Required]
        [StringLength(512)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(512)]
        public string Authors { get; set; } = string.Empty;

        [Range(1, 10_000)]
        public int TotalCopies { get; set; }
    }
}
