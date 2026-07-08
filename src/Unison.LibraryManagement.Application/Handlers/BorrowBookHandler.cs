using System;
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

        private readonly int _defaultDays = 14;

        public BorrowBookHandler(IBookRepository books, IBookCopyRepository copies, ILoanRepository loans)
        {
            _books = books;
            _copies = copies;
            _loans = loans;
        }

        public async Task Handle(BorrowBookCommand command)
        {
            var book = command.BookId.HasValue ? await _books.GetByIdAsync(command.BookId.Value) : await _books.GetByIsbnAsync(command.ISBN ?? string.Empty);
            if (book == null) throw new InvalidOperationException("Book not found");

            var copy = await _copies.GetAvailableCopyAsync(book.Id);
            if (copy == null) throw new InvalidOperationException("No available copies");

            copy.Status = BookCopyStatus.OnLoan;
            _copies.UpdateAsync(copy).GetAwaiter().GetResult();

            var now = DateTime.UtcNow;
            var loan = new Loan { Id = Guid.NewGuid(), BookCopyId = copy.Id, UserId = command.UserId, BorrowedAt = now, DueAt = now.AddDays(_defaultDays) };
            await _loans.AddAsync(loan);

            await _copies.SaveChangesAsync();
            await _loans.SaveChangesAsync();
        }
    }
}
