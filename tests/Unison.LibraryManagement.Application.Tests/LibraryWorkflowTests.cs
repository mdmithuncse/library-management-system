using Unison.LibraryManagement.Application.Commands;
using Unison.LibraryManagement.Application.Handlers;
using Unison.LibraryManagement.Domain.Entities;
using Unison.LibraryManagement.Domain.Repositories;

namespace Unison.LibraryManagement.Application.Tests
{
    public class LibraryWorkflowTests
    {
        [Fact]
        public async Task BorrowBook_marks_copy_on_loan_and_creates_loan()
        {
            var book = new Book { Id = Guid.NewGuid(), ISBN = "978-demo", Title = "Demo", Authors = "Unison", TotalCopies = 1 };
            var copy = new BookCopy { Id = Guid.NewGuid(), BookId = book.Id, Status = BookCopyStatus.Available };
            var books = new InMemoryBookRepository(book);
            var copies = new InMemoryBookCopyRepository(copy);
            var loans = new InMemoryLoanRepository();
            var handler = new BorrowBookHandler(books, copies, loans, new InMemoryUnitOfWork());

            var userId = Guid.NewGuid();
            await handler.Handle(new BorrowBookCommand { UserId = userId, BookId = book.Id });

            Assert.Equal(BookCopyStatus.OnLoan, copy.Status);
            var loan = Assert.Single(loans.Items);
            Assert.Equal(copy.Id, loan.BookCopyId);
            Assert.Equal(userId, loan.UserId);
        }

        [Fact]
        public async Task ReturnBook_rejects_loan_owned_by_another_user()
        {
            var ownerId = Guid.NewGuid();
            var copy = new BookCopy { Id = Guid.NewGuid(), Status = BookCopyStatus.OnLoan };
            var loan = new Loan { Id = Guid.NewGuid(), UserId = ownerId, BookCopyId = copy.Id, BookCopy = copy, BorrowedAt = DateTime.UtcNow.AddDays(-2), DueAt = DateTime.UtcNow.AddDays(12) };
            var handler = new ReturnBookHandler(new InMemoryLoanRepository(loan), new InMemoryBookCopyRepository(copy), new InMemoryFineRepository(), new InMemoryUnitOfWork());

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                handler.Handle(new ReturnBookCommand { UserId = Guid.NewGuid(), LoanId = loan.Id }));

            Assert.Null(loan.ReturnedAt);
            Assert.Equal(BookCopyStatus.OnLoan, copy.Status);
        }

        [Fact]
        public async Task ReturnBook_creates_fine_for_overdue_loan_and_marks_copy_available()
        {
            var userId = Guid.NewGuid();
            var copy = new BookCopy { Id = Guid.NewGuid(), Status = BookCopyStatus.OnLoan };
            var loan = new Loan { Id = Guid.NewGuid(), UserId = userId, BookCopyId = copy.Id, BookCopy = copy, BorrowedAt = DateTime.UtcNow.AddDays(-20), DueAt = DateTime.UtcNow.AddDays(-4) };
            var fines = new InMemoryFineRepository();
            var handler = new ReturnBookHandler(new InMemoryLoanRepository(loan), new InMemoryBookCopyRepository(copy), fines, new InMemoryUnitOfWork());

            await handler.Handle(new ReturnBookCommand { UserId = userId, LoanId = loan.Id });

            Assert.NotNull(loan.ReturnedAt);
            Assert.Equal(BookCopyStatus.Available, copy.Status);
            Assert.Single(fines.Items);
            Assert.True(fines.Items[0].Amount > 0);
        }

        [Fact]
        public async Task CreateBook_rejects_non_positive_copy_count()
        {
            var handler = new CreateBookHandler(new InMemoryBookRepository(), new InMemoryBookCopyRepository());

            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(new CreateBookCommand { ISBN = "978-demo", Title = "Demo", Authors = "Unison", TotalCopies = 0 }));
        }

        private sealed class InMemoryUnitOfWork : IUnitOfWork
        {
            public Task ExecuteInTransactionAsync(Func<Task> operation) => operation();
            public Task SaveChangesAsync() => Task.CompletedTask;
        }

        private sealed class InMemoryBookRepository : IBookRepository
        {
            private readonly List<Book> _books;

            public InMemoryBookRepository(params Book[] books)
            {
                _books = books.ToList();
            }

            public Task AddAsync(Book book)
            {
                _books.Add(book);
                return Task.CompletedTask;
            }

            public Task<Book?> GetByIdAsync(Guid id) => Task.FromResult(_books.FirstOrDefault(x => x.Id == id));

            public Task<Book?> GetByIsbnAsync(string isbn) => Task.FromResult(_books.FirstOrDefault(x => x.ISBN == isbn));

            public Task<Unison.LibraryManagement.Domain.Dto.PagedResult<Book>> SearchAsync(string query, int page, int pageSize)
            {
                return Task.FromResult(new Unison.LibraryManagement.Domain.Dto.PagedResult<Book> { Items = _books, Page = page, PageSize = pageSize, Total = _books.Count });
            }

            public Task UpdateAsync(Book book) => Task.CompletedTask;
            public Task SaveChangesAsync() => Task.CompletedTask;
        }

        private sealed class InMemoryBookCopyRepository : IBookCopyRepository
        {
            private readonly List<BookCopy> _copies;

            public InMemoryBookCopyRepository(params BookCopy[] copies)
            {
                _copies = copies.ToList();
            }

            public Task AddAsync(BookCopy copy)
            {
                _copies.Add(copy);
                return Task.CompletedTask;
            }

            public Task<BookCopy?> GetAvailableCopyAsync(Guid bookId)
            {
                return Task.FromResult(_copies.FirstOrDefault(c => c.BookId == bookId && c.Status == BookCopyStatus.Available));
            }

            public Task<IEnumerable<BookCopy>> GetCopiesAsync(Guid bookId)
            {
                return Task.FromResult<IEnumerable<BookCopy>>(_copies.Where(c => c.BookId == bookId).ToList());
            }

            public Task UpdateAsync(BookCopy copy) => Task.CompletedTask;
            public Task SaveChangesAsync() => Task.CompletedTask;
        }

        private sealed class InMemoryLoanRepository : ILoanRepository
        {
            public List<Loan> Items { get; } = new();

            public InMemoryLoanRepository(params Loan[] loans)
            {
                Items.AddRange(loans);
            }

            public Task AddAsync(Loan loan)
            {
                Items.Add(loan);
                return Task.CompletedTask;
            }

            public Task<Loan?> GetByIdAsync(Guid id) => Task.FromResult(Items.FirstOrDefault(l => l.Id == id));

            public Task<IEnumerable<Loan>> GetActiveLoansByUserAsync(Guid userId)
            {
                return Task.FromResult<IEnumerable<Loan>>(Items.Where(l => l.UserId == userId && l.ReturnedAt == null).ToList());
            }

            public Task<IEnumerable<Loan>> GetOverdueLoansAsync()
            {
                return Task.FromResult<IEnumerable<Loan>>(Items.Where(l => l.ReturnedAt == null && l.DueAt < DateTime.UtcNow).ToList());
            }

            public Task UpdateAsync(Loan loan) => Task.CompletedTask;
            public Task SaveChangesAsync() => Task.CompletedTask;
        }

        private sealed class InMemoryFineRepository : IFineRepository
        {
            public List<Fine> Items { get; } = new();

            public Task AddAsync(Fine fine)
            {
                Items.Add(fine);
                return Task.CompletedTask;
            }

            public Task<IEnumerable<Fine>> GetByUserAsync(Guid userId)
            {
                return Task.FromResult<IEnumerable<Fine>>(Items);
            }

            public Task SaveChangesAsync() => Task.CompletedTask;
        }
    }
}
