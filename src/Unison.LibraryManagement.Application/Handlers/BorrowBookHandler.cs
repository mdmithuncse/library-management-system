using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unison.LibraryManagement.Application.Commands;
using Unison.LibraryManagement.Domain.Entities;
using Unison.LibraryManagement.Domain.Repositories;

namespace Unison.LibraryManagement.Application.Handlers
{
    public class BorrowBookHandler
    {
        private readonly IBookRepository _books;
        private readonly IBookCopyRepository _copies;
        private readonly ILoanRepository _loans;
        private readonly IUnitOfWork _unitOfWork;

        private readonly int _defaultDays = 14;

        public BorrowBookHandler(IBookRepository books, IBookCopyRepository copies, ILoanRepository loans, IUnitOfWork unitOfWork)
        {
            _books = books;
            _copies = copies;
            _loans = loans;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(BorrowBookCommand command)
        {
            if (!command.BookId.HasValue && string.IsNullOrWhiteSpace(command.ISBN))
            {
                throw new ArgumentException("BookId or ISBN is required");
            }

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var book = command.BookId.HasValue
                    ? await _books.GetByIdAsync(command.BookId.Value)
                    : await _books.GetByIsbnAsync(command.ISBN ?? string.Empty);

                if (book == null) throw new KeyNotFoundException("Book not found");

                var copy = await _copies.GetAvailableCopyAsync(book.Id);
                if (copy == null) throw new InvalidOperationException("No available copies");

                copy.Status = BookCopyStatus.OnLoan;
                await _copies.UpdateAsync(copy);

                var now = DateTime.UtcNow;
                var loan = new Loan { Id = Guid.NewGuid(), BookCopyId = copy.Id, UserId = command.UserId, BorrowedAt = now, DueAt = now.AddDays(_defaultDays) };
                await _loans.AddAsync(loan);
            });
        }
    }
}
