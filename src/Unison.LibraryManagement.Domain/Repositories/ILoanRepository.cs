using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unison.LibraryManagement.Domain.Entities;

namespace Unison.LibraryManagement.Domain.Repositories
{
    public interface ILoanRepository
    {
        Task AddAsync(Loan loan);
        Task<Loan?> GetByIdAsync(Guid id);
        Task<IEnumerable<Loan>> GetActiveLoansByUserAsync(Guid userId);
        Task<IEnumerable<Loan>> GetOverdueLoansAsync();
        Task UpdateAsync(Loan loan);
        Task SaveChangesAsync();
    }
}
