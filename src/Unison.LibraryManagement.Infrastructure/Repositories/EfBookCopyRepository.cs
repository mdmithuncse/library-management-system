using Microsoft.EntityFrameworkCore;
using Unison.LibraryManagement.Domain.Entities;
using Unison.LibraryManagement.Domain.Repositories;

namespace Unison.LibraryManagement.Infrastructure.Repositories
{
    public class EfBookCopyRepository : IBookCopyRepository
    {
        private readonly Persistence.LibraryManagementDbContext _db;
        public EfBookCopyRepository(Persistence.LibraryManagementDbContext db) => _db = db;

        public async Task AddAsync(BookCopy copy)
        {
            await _db.BookCopies.AddAsync(copy);
            // SaveChanges moved to explicit SaveChangesAsync
        }

        public async Task<IEnumerable<BookCopy>> GetCopiesAsync(Guid bookId)
        {
            return await _db.BookCopies.Where(c => c.BookId == bookId).ToListAsync();
        }

        public async Task<BookCopy?> GetAvailableCopyAsync(Guid bookId)
        {
            return await _db.BookCopies.FirstOrDefaultAsync(c => c.BookId == bookId && c.Status == BookCopyStatus.Available);
        }

        public async Task UpdateAsync(BookCopy copy)
        {
            _db.BookCopies.Update(copy);
            // SaveChanges moved to explicit SaveChangesAsync
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
