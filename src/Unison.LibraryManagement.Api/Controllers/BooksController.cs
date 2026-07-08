using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Unison.LibraryManagement.Application.Services;
using Unison.LibraryManagement.Application.Dtos;

namespace Unison.LibraryManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly Unison.LibraryManagement.Application.Handlers.SearchBooksHandler _searchHandler;
        private readonly Unison.LibraryManagement.Application.Handlers.CreateBookHandler _createHandler;

        public BooksController(Unison.LibraryManagement.Application.Handlers.SearchBooksHandler searchHandler, Unison.LibraryManagement.Application.Handlers.CreateBookHandler createHandler)
        {
            _searchHandler = searchHandler;
            _createHandler = createHandler;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int size = 20)
        {
            var cmd = new Unison.LibraryManagement.Application.Commands.SearchBooksQuery { Query = q ?? string.Empty, Page = page, PageSize = size };
            var result = await _searchHandler.Handle(cmd);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "librarian,admin")]
        public async Task<IActionResult> Create([FromBody] BookCreateDto dto)
        {
            var cmd = new Unison.LibraryManagement.Application.Commands.CreateBookCommand { ISBN = dto.ISBN, Title = dto.Title, Authors = dto.Authors, TotalCopies = dto.TotalCopies };
            await _createHandler.Handle(cmd);
            return CreatedAtAction(nameof(Search), new { isbn = dto.ISBN }, dto);
        }
    }
}
