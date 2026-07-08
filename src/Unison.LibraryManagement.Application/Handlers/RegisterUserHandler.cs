using Unison.LibraryManagement.Application.Security;
using Unison.LibraryManagement.Domain.Entities;
using Unison.LibraryManagement.Domain.Repositories;

namespace Unison.LibraryManagement.Application.Handlers
{
    public class RegisterUserHandler
    {
        private readonly IUserRepository _users;
        private readonly IRoleRepository _roles;
        private readonly IPasswordHasher _hasher;

        public RegisterUserHandler(IUserRepository users, IRoleRepository roles, IPasswordHasher hasher)
        {
            _users = users;
            _roles = roles;
            _hasher = hasher;
        }

        public async Task Handle(Commands.RegisterUserCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Email)) throw new ArgumentException("Email is required");
            if (string.IsNullOrWhiteSpace(command.Password)) throw new ArgumentException("Password is required");

            var existing = await _users.GetByEmailAsync(command.Email);
            if (existing != null) throw new InvalidOperationException("A user with the specified email already exists.");

            var hash = _hasher.Hash(command.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = command.Email,
                PasswordHash = hash.HashBase64,
                PasswordSalt = hash.SaltBase64,
                PasswordIterations = hash.Iterations,
                CreatedAt = DateTime.UtcNow
            };

            var memberRole = await _roles.GetByNameAsync("member");
            if (memberRole == null) throw new InvalidOperationException("Default role 'member' not found.");

            user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = memberRole.Id });

            await _users.AddAsync(user);
            await _users.SaveChangesAsync();
        }
    }
}
