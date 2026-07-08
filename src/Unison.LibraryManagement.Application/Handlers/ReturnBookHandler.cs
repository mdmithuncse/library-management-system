using System;
using System.Collections.Generic;
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
        private readonly IUnitOfWork _unitOfWork;

        public ReturnBookHandler(ILoanRepository loans, IBookCopyRepository copies, IFineRepository fines, IUnitOfWork unitOfWork)
        {
            _loans = loans;
            _copies = copies;
            _fines = fines;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ReturnBookCommand command)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var loan = await _loans.GetByIdAsync(command.LoanId);
                if (loan == null) throw new KeyNotFoundException("Loan not found");
                if (loan.UserId != command.UserId) throw new UnauthorizedAccessException("You cannot return another user's loan");
                if (loan.ReturnedAt != null) throw new InvalidOperationException("Already returned");

                loan.ReturnedAt = DateTime.UtcNow;

                // Fine policy: 0.5 per day overdue after a 1-day grace period.
                var daysLate = (loan.ReturnedAt.Value.Date - loan.DueAt.Date).Days - 1;
                if (daysLate > 0)
                {
                    var amount = daysLate * 0.5m;
                    var fine = new Domain.Entities.Fine { Id = Guid.NewGuid(), LoanId = loan.Id, Amount = amount, CreatedAt = DateTime.UtcNow };
                    await _fines.AddAsync(fine);
                }

                var copy = loan.BookCopy;
                if (copy != null)
                {
                    copy.Status = Domain.Entities.BookCopyStatus.Available;
                    await _copies.UpdateAsync(copy);
                }

                await _loans.UpdateAsync(loan);
            });
        }
    }
}
