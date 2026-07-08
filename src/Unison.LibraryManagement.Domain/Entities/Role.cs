using System.Collections.Generic;

namespace Unison.LibraryManagement.Domain.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        // Navigation
        public List<UserRole> UserRoles { get; set; } = new();
    }
}
