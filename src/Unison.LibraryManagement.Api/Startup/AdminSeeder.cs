using Unison.LibraryManagement.Application.Security;
using Unison.LibraryManagement.Domain.Entities;
using Unison.LibraryManagement.Domain.Repositories;

namespace Unison.LibraryManagement.Api.Startup
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var provider = scope.ServiceProvider;
            var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger("AdminSeeder");

            try
            {
                var email = Environment.GetEnvironmentVariable("INITIAL_ADMIN_EMAIL");
                var password = Environment.GetEnvironmentVariable("INITIAL_ADMIN_PASSWORD");
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    logger.LogInformation("INITIAL_ADMIN_EMAIL or INITIAL_ADMIN_PASSWORD not provided; skipping admin seeding.");
                    return;
                }

                var users = provider.GetRequiredService<IUserRepository>();
                var roles = provider.GetRequiredService<IRoleRepository>();
                var hasher = provider.GetRequiredService<IPasswordHasher>();

                var existing = await users.GetByEmailAsync(email);
                if (existing != null)
                {
                    logger.LogInformation("Admin user already exists: {Email}", email);
                    return;
                }

                var adminRole = await roles.GetByNameAsync("admin");
                if (adminRole == null)
                {
                    logger.LogWarning("Admin role not found; seeding cannot proceed.");
                    return;
                }

                var hash = hasher.Hash(password);
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    PasswordHash = hash.HashBase64,
                    PasswordSalt = hash.SaltBase64,
                    PasswordIterations = hash.Iterations,
                    CreatedAt = DateTime.UtcNow
                };
                user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = adminRole.Id });

                await users.AddAsync(user);
                await users.SaveChangesAsync();

                logger.LogInformation("Admin user created: {Email}", email);
            }
            catch (Exception ex)
            {
                var logger2 = services.GetRequiredService<ILoggerFactory>().CreateLogger("AdminSeeder");
                logger2.LogError(ex, "Error seeding admin user");
            }
        }
    }
}
