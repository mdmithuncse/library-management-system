using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Unison.LibraryManagement.Domain.Entities;
using Unison.LibraryManagement.Domain.Repositories;
using Unison.LibraryManagement.Infrastructure.Persistence;

namespace Unison.LibraryManagement.Infrastructure.Repositories
{
    public class EfUserRepository : IUserRepository
    {
        private readonly LibraryManagementDbContext _db;

        public EfUserRepository(LibraryManagementDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(User user)
        {
            await _db.Users.AddAsync(user);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _db.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _db.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
