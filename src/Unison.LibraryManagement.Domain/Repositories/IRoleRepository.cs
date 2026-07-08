using Unison.LibraryManagement.Domain.Entities;

namespace Unison.LibraryManagement.Domain.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string name);
        Task<Role?> GetByIdAsync(int id);
        Task AddAsync(Role role);
        Task<IEnumerable<Role>> ListAsync();
        Task SaveChangesAsync();
    }
}
