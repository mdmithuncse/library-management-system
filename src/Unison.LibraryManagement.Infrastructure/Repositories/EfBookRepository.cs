using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Unison.LibraryManagement.Domain.Entities;
using Unison.LibraryManagement.Domain.Repositories;
using Unison.LibraryManagement.Domain.Dto;

namespace Unison.LibraryManagement.Infrastructure.Repositories
{
    public class EfBookRepository : IBookRepository
    {
        private readonly Persistence.LibraryManagementDbContext _db;
        public EfBookRepository(Persistence.LibraryManagementDbContext db) => _db = db;

        public async Task AddAsync(Book book)
        {
            await _db.Books.AddAsync(book);
            // SaveChanges moved to explicit SaveChangesAsync
        }

        public async Task<Book?> GetByIdAsync(Guid id) => await _db.Books.Include(b => b.Copies).FirstOrDefaultAsync(x => x.Id == id);

        public async Task<Book?> GetByIsbnAsync(string isbn) => await _db.Books.Include(b => b.Copies).FirstOrDefaultAsync(x => x.ISBN == isbn);

        public async Task UpdateAsync(Book book)
        {
            _db.Books.Update(book);
            // SaveChanges moved to explicit SaveChangesAsync
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task<PagedResult<Book>> SearchAsync(string query, int page, int pageSize)
        {
            var q = _db.Books.AsQueryable();
            if (!string.IsNullOrWhiteSpace(query))
            {
                q = q.Where(b => EF.Functions.Like(b.Title, $"%{query}%") || EF.Functions.Like(b.Authors, $"%{query}%") || EF.Functions.Like(b.ISBN, $"%{query}%"));
            }

            var total = await q.LongCountAsync();
            var items = await q.OrderBy(b => b.Title).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<Book> { Items = items, Page = page, PageSize = pageSize, Total = total };
        }
    }
}
