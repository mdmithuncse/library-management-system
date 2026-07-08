using System;

namespace Unison.LibraryManagement.Application.Commands
{
    public class RegisterUserCommand
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
