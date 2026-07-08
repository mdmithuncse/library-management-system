using System.Linq;
using System.Threading.Tasks;
using Unison.LibraryManagement.Application.Commands;
using Unison.LibraryManagement.Application.Dtos;
using Unison.LibraryManagement.Domain.Repositories;
using Unison.LibraryManagement.Application.Mappings;

namespace Unison.LibraryManagement.Application.Handlers
{
    public class SearchBooksHandler
    {
        private readonly IBookRepository _books;

        public SearchBooksHandler(IBookRepository books)
        {
            _books = books;
        }

        public async Task<PagedResult<BookDto>> Handle(SearchBooksQuery query)
        {
            var domainResult = await _books.SearchAsync(query.Query ?? string.Empty, query.Page, query.PageSize);

            return domainResult.ToPagedResult(b => b.ToDto());
        }
    }
}
