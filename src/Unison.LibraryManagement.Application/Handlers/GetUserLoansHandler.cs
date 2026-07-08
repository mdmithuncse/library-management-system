using System.Linq;
using System.Threading.Tasks;
using Unison.LibraryManagement.Application.Commands;
using Unison.LibraryManagement.Application.Dtos;
using Unison.LibraryManagement.Domain.Repositories;
using Unison.LibraryManagement.Application.Mappings;

namespace Unison.LibraryManagement.Application.Handlers
{
    public class GetUserLoansHandler
    {
        private readonly ILoanRepository _loans;

        public GetUserLoansHandler(ILoanRepository loans)
        {
            _loans = loans;
        }

        public async Task<System.Collections.Generic.IEnumerable<LoanDto>> Handle(GetUserLoansQuery query)
        {
            var loans = await _loans.GetActiveLoansByUserAsync(query.UserId);
            return loans.Select(l => l.ToDto());
        }
    }
}
