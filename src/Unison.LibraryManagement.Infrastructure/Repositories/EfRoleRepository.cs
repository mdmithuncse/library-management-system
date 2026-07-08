using Microsoft.EntityFrameworkCore;
using Unison.LibraryManagement.Domain.Entities;
using Unison.LibraryManagement.Domain.Repositories;
using Unison.LibraryManagement.Infrastructure.Persistence;

namespace Unison.LibraryManagement.Infrastructure.Repositories
{
    public class EfRoleRepository : IRoleRepository
    {
        private readonly LibraryManagementDbContext _db;

        public EfRoleRepository(LibraryManagementDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Role role)
        {
            await _db.Roles.AddAsync(role);
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _db.Roles.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _db.Roles.FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task<IEnumerable<Role>> ListAsync()
        {
            return await _db.Roles.ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
