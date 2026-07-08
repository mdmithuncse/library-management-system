using Unison.LibraryManagement.Domain.Entities;

namespace Unison.LibraryManagement.Domain.Repositories
{
    public interface IBookCopyRepository
    {
        Task<BookCopy?> GetAvailableCopyAsync(Guid bookId);
        Task<IEnumerable<BookCopy>> GetCopiesAsync(Guid bookId);
        Task UpdateAsync(BookCopy copy);
        Task AddAsync(BookCopy copy);
        Task SaveChangesAsync();
    }
}
