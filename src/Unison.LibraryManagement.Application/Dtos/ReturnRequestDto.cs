using System.ComponentModel.DataAnnotations;

namespace Unison.LibraryManagement.Application.Dtos
{
    public class ReturnRequestDto
    {
        [Required]
        public Guid LoanId { get; set; }
    }
}
