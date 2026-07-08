using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Unison.LibraryManagement.Application.Security;
using Unison.LibraryManagement.Domain.Repositories;

namespace Unison.LibraryManagement.Api.Auth
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserRepository _users;
        private readonly IPasswordHasher _hasher;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IUserRepository users,
            IPasswordHasher hasher)
            : base(options, logger, encoder)
        {
            _users = users;
            _hasher = hasher;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.NoResult();

            try
            {
                var header = AuthenticationHeaderValue.Parse(Request.Headers.Authorization.ToString());
                if (!"Basic".Equals(header.Scheme, StringComparison.OrdinalIgnoreCase))
                    return AuthenticateResult.NoResult();

                var credentialBytes = Convert.FromBase64String(header.Parameter ?? string.Empty);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
                if (credentials.Length != 2) return AuthenticateResult.Fail("Invalid Authorization header");

                var email = credentials[0];
                var password = credentials[1];

                var user = await _users.GetByEmailAsync(email);
                if (user == null) return AuthenticateResult.Fail("Invalid username or password");

                if (!_hasher.Verify(password, user.PasswordSalt, user.PasswordHash, user.PasswordIterations))
                    return AuthenticateResult.Fail("Invalid username or password");

                var claims = new[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Email)
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);

                // add role claims
                foreach (var ur in user.UserRoles)
                {
                    if (ur.Role != null)
                    {
                        ((ClaimsIdentity)identity).AddClaim(new Claim(ClaimTypes.Role, ur.Role.Name));
                    }
                }

                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error authenticating");
                return AuthenticateResult.Fail("Invalid Authorization header");
            }
        }
    }
}
