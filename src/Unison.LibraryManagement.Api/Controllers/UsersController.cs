using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unison.LibraryManagement.Application.Commands;
using Unison.LibraryManagement.Application.Handlers;

namespace Unison.LibraryManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AssignRoleHandler _assignRoleHandler;

        public UsersController(AssignRoleHandler assignRoleHandler)
        {
            _assignRoleHandler = assignRoleHandler;
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}/roles")]
        public async Task<IActionResult> AssignRole([FromRoute] System.Guid id, [FromBody] AssignRoleCommand command)
        {
            if (id != command.UserId) return BadRequest();
            await _assignRoleHandler.Handle(command);
            return NoContent();
        }
    }
}
