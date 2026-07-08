using Unison.LibraryManagement.Domain.Entities;

namespace Unison.LibraryManagement.Application.Services
{
    public interface IFineService
    {
        decimal CalculateFine(Loan loan, decimal dailyRate, int graceDays);
        Task<decimal> GetOutstandingForUserAsync(Guid userId);
    }
}
