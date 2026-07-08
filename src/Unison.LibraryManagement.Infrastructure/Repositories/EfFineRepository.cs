using Microsoft.EntityFrameworkCore;
using Unison.LibraryManagement.Domain.Entities;
using Unison.LibraryManagement.Domain.Repositories;

namespace Unison.LibraryManagement.Infrastructure.Repositories
{
    public class EfFineRepository : IFineRepository
    {
        private readonly Persistence.LibraryManagementDbContext _db;
        public EfFineRepository(Persistence.LibraryManagementDbContext db) => _db = db;

        public async Task AddAsync(Fine fine)
        {
            await _db.Fines.AddAsync(fine);
            // SaveChanges moved to explicit SaveChangesAsync
        }

        public async Task<IEnumerable<Fine>> GetByUserAsync(Guid userId)
        {
            // Join loans -> fines
            var fines = from f in _db.Fines
                        join l in _db.Loans on f.LoanId equals l.Id
                        where l.UserId == userId
                        select f;

            return await fines.ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
