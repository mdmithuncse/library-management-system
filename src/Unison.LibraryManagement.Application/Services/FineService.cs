using Unison.LibraryManagement.Domain.Entities;
using Unison.LibraryManagement.Domain.Repositories;

namespace Unison.LibraryManagement.Application.Services
{
    public class FineService : IFineService
    {
        private readonly IFineRepository _fines;
        private readonly ILoanRepository _loans;

        public FineService(IFineRepository fines, ILoanRepository loans)
        {
            _fines = fines;
            _loans = loans;
        }

        public decimal CalculateFine(Loan loan, decimal dailyRate, int graceDays)
        {
            var reference = loan.ReturnedAt ?? DateTime.UtcNow;
            var daysLate = (reference.Date - loan.DueAt.Date).Days - graceDays;
            if (daysLate <= 0) return 0m;
            return (decimal)daysLate * dailyRate;
        }

        public async Task<decimal> GetOutstandingForUserAsync(Guid userId)
        {
            var loans = await _loans.GetActiveLoansByUserAsync(userId);
            decimal total = 0m;
            foreach (var loan in loans)
            {
                if (loan.DueAt < DateTime.UtcNow)
                {
                    var days = (DateTime.UtcNow.Date - loan.DueAt.Date).Days;
                    total += days * 0.5m; // default if no config; controllers may pass configured rate
                }
            }
            return total;
        }
    }
}
