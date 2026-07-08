using Microsoft.AspNetCore.Mvc;
using Unison.LibraryManagement.Application.Commands;
using Unison.LibraryManagement.Application.Handlers;

namespace Unison.LibraryManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly RegisterUserHandler _registerHandler;

        public AuthController(RegisterUserHandler registerHandler)
        {
            _registerHandler = registerHandler;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            await _registerHandler.Handle(command);
            return Created(string.Empty, null);
        }
    }
}
