using System.ComponentModel.DataAnnotations;

namespace Unison.LibraryManagement.Application.Commands
{
    public class AssignRoleCommand
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(64)]
        public string RoleName { get; set; } = null!;
    }
}
