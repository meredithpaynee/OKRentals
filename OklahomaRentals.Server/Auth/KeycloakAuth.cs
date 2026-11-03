using System.Security.Claims;
using System.Text.Json;

namespace OklahomaRentals.Server.Auth
{
    public static class AuthPolicies
    {
        // Staff. Also the default for [Authorize] and for endpoints with no attribute.
        public const string Admin = "Admin";

        // Any signed-in user, e.g. a tenant reading their own data.
        public const string SignedIn = "SignedIn";

        public const string AdminRole = "admin";
    }

    public static class KeycloakClaims
    {
        // Keycloak puts realm roles in a JSON claim: "realm_access": { "roles": [...] }.
        // Copy them into standard role claims so [Authorize(Roles = ...)] and
        // RequireRole work.
        public static void AddRealmRoles(ClaimsPrincipal? principal)
        {
            if (principal?.Identity is not ClaimsIdentity identity) return;

            var realmAccess = identity.FindFirst("realm_access")?.Value;
            if (string.IsNullOrEmpty(realmAccess)) return;

            using var doc = JsonDocument.Parse(realmAccess);
            if (!doc.RootElement.TryGetProperty("roles", out var roles) || roles.ValueKind != JsonValueKind.Array) return;

            foreach (var role in roles.EnumerateArray())
            {
                var name = role.GetString();
                if (!string.IsNullOrEmpty(name) && !identity.HasClaim(identity.RoleClaimType, name))
                    identity.AddClaim(new Claim(identity.RoleClaimType, name));
            }
        }

        // The user's email, only if Keycloak marked it verified. Used to find
        // the tenant record that belongs to a login.
        public static string? VerifiedEmail(ClaimsPrincipal user)
        {
            var verified = user.FindFirst("email_verified")?.Value;
            if (!string.Equals(verified, "true", StringComparison.OrdinalIgnoreCase)) return null;

            return user.FindFirst(ClaimTypes.Email)?.Value ?? user.FindFirst("email")?.Value;
        }

        public static bool IsAdmin(ClaimsPrincipal user) => user.IsInRole(AuthPolicies.AdminRole);
    }
}
