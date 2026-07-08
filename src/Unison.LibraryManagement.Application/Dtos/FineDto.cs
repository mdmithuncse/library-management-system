using System;

namespace Unison.LibraryManagement.Application.Dtos
{
    public class FineDto
    {
        public Guid Id { get; set; }
        public Guid LoanId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
