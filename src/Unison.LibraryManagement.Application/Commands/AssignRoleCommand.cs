using System;

namespace Unison.LibraryManagement.Application.Commands
{
    public class AssignRoleCommand
    {
        public Guid UserId { get; set; }
        public string RoleName { get; set; } = null!;
    }
}
