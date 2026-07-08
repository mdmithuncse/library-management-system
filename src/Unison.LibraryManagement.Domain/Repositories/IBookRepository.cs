using System;
using System.Threading.Tasks;
using Unison.LibraryManagement.Domain.Entities;
using Unison.LibraryManagement.Domain.Dto;

namespace Unison.LibraryManagement.Domain.Repositories
{
    public interface IBookRepository
    {
        Task<Book?> GetByIdAsync(Guid id);
        Task<Book?> GetByIsbnAsync(string isbn);
        Task<PagedResult<Book>> SearchAsync(string query, int page, int pageSize);
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task SaveChangesAsync();
    }
}
