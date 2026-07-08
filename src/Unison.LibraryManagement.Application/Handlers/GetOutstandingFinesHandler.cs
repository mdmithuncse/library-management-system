using Unison.LibraryManagement.Application.Commands;
using Unison.LibraryManagement.Application.Services;

namespace Unison.LibraryManagement.Application.Handlers
{
    public class GetOutstandingFinesHandler
    {
        private readonly IFineService _fineService;

        public GetOutstandingFinesHandler(IFineService fineService)
        {
            _fineService = fineService;
        }

        public async Task<object> Handle(GetOutstandingFinesQuery query)
        {
            var amount = await _fineService.GetOutstandingForUserAsync(query.UserId);
            return new { UserId = query.UserId, Outstanding = amount };
        }
    }
}
