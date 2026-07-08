using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Unison.LibraryManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinesController : ControllerBase
    {
        private readonly Unison.LibraryManagement.Application.Handlers.GetOutstandingFinesHandler _handler;

        public FinesController(Unison.LibraryManagement.Application.Handlers.GetOutstandingFinesHandler handler) => _handler = handler;

        [HttpGet("users/{id}/outstanding")]
        [Authorize]
        public async Task<IActionResult> GetOutstanding(Guid id)
        {
            var caller = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            if (caller != id && !User.IsInRole("librarian") && !User.IsInRole("admin")) return Forbid();

            var cmd = new Unison.LibraryManagement.Application.Commands.GetOutstandingFinesQuery { UserId = id };
            var result = await _handler.Handle(cmd);
            return Ok(result);
        }
    }
}
