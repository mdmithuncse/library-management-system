using System;
using System.ComponentModel.DataAnnotations;

namespace Unison.LibraryManagement.Application.Commands
{
    public class RegisterUserCommand
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = null!;
    }
}
