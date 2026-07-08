using System;
using System.Threading.Tasks;
using Unison.LibraryManagement.Application.Commands;
using Unison.LibraryManagement.Domain.Entities;
using Unison.LibraryManagement.Domain.Repositories;

namespace Unison.LibraryManagement.Application.Handlers
{
    public class CreateBookHandler
    {
        private readonly IBookRepository _books;
        private readonly IBookCopyRepository _copies;

        public CreateBookHandler(IBookRepository books, IBookCopyRepository copies)
        {
            _books = books;
            _copies = copies;
        }

        public async Task Handle(CreateBookCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.ISBN)) throw new ArgumentException("ISBN required");
            if (string.IsNullOrWhiteSpace(command.Title)) throw new ArgumentException("Title required");

            var existing = await _books.GetByIsbnAsync(command.ISBN);
            if (existing != null) throw new InvalidOperationException("Book with same ISBN exists");

            var book = new Book { Id = Guid.NewGuid(), ISBN = command.ISBN, Title = command.Title, Authors = command.Authors, TotalCopies = command.TotalCopies };
            await _books.AddAsync(book);

            for (int i = 0; i < command.TotalCopies; i++)
            {
                var copy = new BookCopy { Id = Guid.NewGuid(), BookId = book.Id, CopyNumber = $"C-{i + 1}" };
                await _copies.AddAsync(copy);
            }

            await _books.SaveChangesAsync();
            await _copies.SaveChangesAsync();
        }
    }
}
