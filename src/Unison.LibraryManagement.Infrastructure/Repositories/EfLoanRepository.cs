using Microsoft.EntityFrameworkCore;
using Unison.LibraryManagement.Domain.Entities;
using Unison.LibraryManagement.Domain.Repositories;

namespace Unison.LibraryManagement.Infrastructure.Repositories
{
    public class EfLoanRepository : ILoanRepository
    {
        private readonly Persistence.LibraryManagementDbContext _db;
        public EfLoanRepository(Persistence.LibraryManagementDbContext db) => _db = db;

        public async Task AddAsync(Loan loan)
        {
            await _db.Loans.AddAsync(loan);
            // SaveChanges moved to explicit SaveChangesAsync
        }

        public async Task<Loan?> GetByIdAsync(Guid id) => await _db.Loans.Include(l => l.BookCopy).FirstOrDefaultAsync(l => l.Id == id);

        public async Task<IEnumerable<Loan>> GetActiveLoansByUserAsync(Guid userId)
        {
            return await _db.Loans.Where(l => l.UserId == userId && l.ReturnedAt == null).ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetOverdueLoansAsync()
        {
            var now = DateTime.UtcNow;
            return await _db.Loans.Where(l => l.ReturnedAt == null && l.DueAt < now).Include(l => l.BookCopy).ToListAsync();
        }

        public async Task UpdateAsync(Loan loan)
        {
            _db.Loans.Update(loan);
            // SaveChanges moved to explicit SaveChangesAsync
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
