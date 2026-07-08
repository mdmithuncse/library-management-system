using System.ComponentModel.DataAnnotations;

namespace Unison.LibraryManagement.Application.Dtos
{
    public class BorrowRequestDto : IValidatableObject
    {
        public Guid? BookId { get; set; }

        [StringLength(32)]
        public string? ISBN { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!BookId.HasValue && string.IsNullOrWhiteSpace(ISBN))
            {
                yield return new ValidationResult("BookId or ISBN is required.", new[] { nameof(BookId), nameof(ISBN) });
            }
        }
    }
}
