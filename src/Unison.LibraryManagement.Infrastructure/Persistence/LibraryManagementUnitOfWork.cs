using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Unison.LibraryManagement.Domain.Repositories;

namespace Unison.LibraryManagement.Infrastructure.Persistence
{
    public class LibraryManagementUnitOfWork : IUnitOfWork
    {
        private readonly LibraryManagementDbContext _db;

        public LibraryManagementUnitOfWork(LibraryManagementDbContext db)
        {
            _db = db;
        }

        public async Task ExecuteInTransactionAsync(Func<Task> operation)
        {
            await using IDbContextTransaction transaction = await _db.Database.BeginTransactionAsync();
            await operation();
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public Task SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}
