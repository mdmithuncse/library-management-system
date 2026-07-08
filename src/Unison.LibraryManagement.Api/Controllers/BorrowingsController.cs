using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unison.LibraryManagement.Application.Dtos;
using Unison.LibraryManagement.Domain.Repositories;

namespace Unison.LibraryManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowingsController : ControllerBase
    {
        private readonly Unison.LibraryManagement.Application.Handlers.BorrowBookHandler _borrowHandler;
        private readonly Unison.LibraryManagement.Application.Handlers.ReturnBookHandler _returnHandler;
        private readonly Unison.LibraryManagement.Application.Handlers.GetUserLoansHandler _getUserLoansHandler;
        private readonly Unison.LibraryManagement.Application.Handlers.GetOverdueLoansHandler _getOverdueHandler;

        public BorrowingsController(Unison.LibraryManagement.Application.Handlers.BorrowBookHandler borrowHandler, Unison.LibraryManagement.Application.Handlers.ReturnBookHandler returnHandler, Unison.LibraryManagement.Application.Handlers.GetUserLoansHandler getUserLoansHandler, Unison.LibraryManagement.Application.Handlers.GetOverdueLoansHandler getOverdueHandler)
        {
            _borrowHandler = borrowHandler;
            _returnHandler = returnHandler;
            _getUserLoansHandler = getUserLoansHandler;
            _getOverdueHandler = getOverdueHandler;
        }

        [HttpPost("borrow")]
        [Authorize]
        public async Task<IActionResult> Borrow([FromBody] BorrowRequestDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var cmd = new Unison.LibraryManagement.Application.Commands.BorrowBookCommand { UserId = userId, BookId = dto.BookId, ISBN = dto.ISBN };
            await _borrowHandler.Handle(cmd);
            return Ok();
        }

        [HttpPost("return")]
        [Authorize]
        public async Task<IActionResult> Return([FromBody] ReturnRequestDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var cmd = new Unison.LibraryManagement.Application.Commands.ReturnBookCommand { UserId = userId, LoanId = dto.LoanId };
            await _returnHandler.Handle(cmd);
            return Ok();
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> MyLoans()
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var cmd = new Unison.LibraryManagement.Application.Commands.GetUserLoansQuery { UserId = userId };
            var loans = await _getUserLoansHandler.Handle(cmd);
            return Ok(loans);
        }

        [HttpGet("overdue")]
        [Authorize(Roles = "librarian,admin")]
        public async Task<IActionResult> Overdue()
        {
            var cmd = new Unison.LibraryManagement.Application.Commands.GetOverdueLoansQuery();
            var loans = await _getOverdueHandler.Handle(cmd);
            return Ok(loans);
        }
    }
}
