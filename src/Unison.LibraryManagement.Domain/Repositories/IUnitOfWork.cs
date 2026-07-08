using System;
using System.Threading.Tasks;

namespace Unison.LibraryManagement.Domain.Repositories
{
    public interface IUnitOfWork
    {
        Task ExecuteInTransactionAsync(Func<Task> operation);
        Task SaveChangesAsync();
    }
}
