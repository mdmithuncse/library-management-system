using System;
using System.Collections.Generic;

namespace Unison.LibraryManagement.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string PasswordSalt { get; set; } = null!;
        public int PasswordIterations { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public List<UserRole> UserRoles { get; set; } = new();
    }
}
