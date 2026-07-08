using System;

namespace Unison.LibraryManagement.Application.Commands
{
    public class ReturnBookCommand
    {
        public Guid UserId { get; set; }
        public Guid LoanId { get; set; }
    }
}
