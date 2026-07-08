using System.Threading.Tasks;

namespace Unison.LibraryManagement.Application.Security
{
    public record PasswordHashResult(string HashBase64, string SaltBase64, int Iterations);

    public interface IPasswordHasher
    {
        PasswordHashResult Hash(string password);
        bool Verify(string password, string saltBase64, string hashBase64, int iterations);
    }
}
