using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unison.LibraryManagement.Domain.Entities;

namespace Unison.LibraryManagement.Domain.Repositories
{
    public interface IFineRepository
    {
        Task AddAsync(Fine fine);
        Task<IEnumerable<Fine>> GetByUserAsync(Guid userId);
        Task SaveChangesAsync();
    }
}
