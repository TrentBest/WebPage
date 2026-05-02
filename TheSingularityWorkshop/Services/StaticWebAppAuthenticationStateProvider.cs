using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace TheSingularityWorkshop.Services
{
    public class StaticWebAppAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _http;
        public StaticWebAppAuthenticationStateProvider(HttpClient http) => _http = http;

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var data = await _http.GetFromJsonAsync<ClientPrincipalData>("/.auth/me");
                if (data?.ClientPrincipal == null)
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

                var principal = data.ClientPrincipal;
                var identity = new ClaimsIdentity(principal.IdentityProvider);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, principal.UserId));
                identity.AddClaim(new Claim(ClaimTypes.Name, principal.UserDetails));

                foreach (var role in principal.UserRoles)
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));

                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }
    }

    public class ClientPrincipalData { public ClientPrincipal? ClientPrincipal { get; set; } }
    public class ClientPrincipal
    {
        public string IdentityProvider { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserDetails { get; set; } = string.Empty;
        public IEnumerable<string> UserRoles { get; set; } = new List<string>();
    }
}