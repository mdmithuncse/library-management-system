using Unison.LibraryManagement.Domain.Entities;
using Unison.LibraryManagement.Domain.Repositories;

namespace Unison.LibraryManagement.Application.Handlers
{
    public class AssignRoleHandler
    {
        private readonly IUserRepository _users;
        private readonly IRoleRepository _roles;

        public AssignRoleHandler(IUserRepository users, IRoleRepository roles)
        {
            _users = users;
            _roles = roles;
        }

        public async Task Handle(Commands.AssignRoleCommand command)
        {
            var user = await _users.GetByIdAsync(command.UserId);
            if (user == null) throw new InvalidOperationException("User not found");

            var role = await _roles.GetByNameAsync(command.RoleName);
            if (role == null) throw new InvalidOperationException("Role not found");

            if (user.UserRoles.Any(ur => ur.RoleId == role.Id))
            {
                // already assigned
                return;
            }

            user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
            await _users.SaveChangesAsync();
        }
    }
}
