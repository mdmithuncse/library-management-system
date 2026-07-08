namespace Unison.LibraryManagement.Application.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public IEnumerable<string> Roles { get; set; } = Array.Empty<string>();
    }
}
