using Unison.LibraryManagement.Application.Dtos;

namespace Unison.LibraryManagement.Application.Mappings
{
    public static class MappingExtensions
    {
        public static BookDto ToDto(this Unison.LibraryManagement.Domain.Entities.Book b)
        {
            if (b == null) return null!;
            return new BookDto
            {
                Id = b.Id,
                ISBN = b.ISBN,
                Title = b.Title,
                Authors = b.Authors,
                TotalCopies = b.TotalCopies
            };
        }

        public static LoanDto ToDto(this Unison.LibraryManagement.Domain.Entities.Loan l)
        {
            if (l == null) return null!;
            return new LoanDto
            {
                Id = l.Id,
                BookCopyId = l.BookCopyId,
                UserId = l.UserId,
                BorrowedAt = l.BorrowedAt,
                DueAt = l.DueAt,
                ReturnedAt = l.ReturnedAt,
                FineAmountPaid = l.FineAmountPaid
            };
        }

        public static FineDto ToDto(this Unison.LibraryManagement.Domain.Entities.Fine f)
        {
            if (f == null) return null!;
            return new FineDto
            {
                Id = f.Id,
                LoanId = f.LoanId,
                Amount = f.Amount,
                CreatedAt = f.CreatedAt,
                PaidAt = f.PaidAt
            };
        }

        public static PagedResult<TTarget> ToPagedResult<TSource, TTarget>(this Unison.LibraryManagement.Domain.Dto.PagedResult<TSource> src, Func<TSource, TTarget> map)
        {
            if (src == null) return new PagedResult<TTarget>();
            return new PagedResult<TTarget>
            {
                Page = src.Page,
                PageSize = src.PageSize,
                Total = src.Total,
                Items = src.Items.Select(map).ToList()
            };
        }
    }
}
