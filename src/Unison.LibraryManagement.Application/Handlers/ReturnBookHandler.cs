using System;
using System.Threading.Tasks;
using Unison.LibraryManagement.Application.Commands;
using Unison.LibraryManagement.Domain.Repositories;

namespace Unison.LibraryManagement.Application.Handlers
{
    public class ReturnBookHandler
    {
        private readonly ILoanRepository _loans;
        private readonly IBookCopyRepository _copies;
        private readonly IFineRepository _fines;

        public ReturnBookHandler(ILoanRepository loans, IBookCopyRepository copies, IFineRepository fines)
        {
            _loans = loans;
            _copies = copies;
            _fines = fines;
        }

        public async Task Handle(ReturnBookCommand command)
        {
            var loan = await _loans.GetByIdAsync(command.LoanId);
            if (loan == null) throw new InvalidOperationException("Loan not found");
            if (loan.ReturnedAt != null) throw new InvalidOperationException("Already returned");

            loan.ReturnedAt = DateTime.UtcNow;

            // compute fine simple: 0.5 per day overdue, 1 day grace
            var daysLate = (loan.ReturnedAt.Value.Date - loan.DueAt.Date).Days - 1;
            if (daysLate > 0)
            {
                var amount = daysLate * 0.5m;
                var fine = new Domain.Entities.Fine { Id = Guid.NewGuid(), LoanId = loan.Id, Amount = amount, CreatedAt = DateTime.UtcNow };
                await _fines.AddAsync(fine);
                await _fines.SaveChangesAsync();
            }

            // mark copy available
            var copy = loan.BookCopy;
            if (copy != null)
            {
                copy.Status = Domain.Entities.BookCopyStatus.Available;
                await _copies.UpdateAsync(copy);
                await _copies.SaveChangesAsync();
            }

            await _loans.UpdateAsync(loan);
            await _loans.SaveChangesAsync();
        }
    }
}
