using FluentBlazorRouter;
using smooms.app.Utils;

namespace smooms.app.Authentication;

public interface IAuthenticationService
{
    bool IsAuthenticated { get; internal set; }
    IReadOnlySet<string> Roles { get; internal set; }
    bool IsAuthorizedFor(Type pageType);

    bool HasRoles(IEnumerable<string> requiredRoles);
}

public class AuthenticationService : IAuthenticationService
{
    private IRouteProvider _routeProvider;

    private bool _isAuthenticated;
    private IReadOnlySet<string> _roles = new HashSet<string>();

    public AuthenticationService(IRouteProvider routeProvider)
    {
        _routeProvider = routeProvider;
    }

    IReadOnlySet<string> IAuthenticationService.Roles
    {
        get => _roles;
        set => _roles = value;
    }

    bool IAuthenticationService.IsAuthenticated
    {
        get => _isAuthenticated;
        set => _isAuthenticated = value;
    }

    public bool IsAuthorizedFor(Type pageType)
    {
        var (authenticationRequired, roles) = GetAuthenticationData(pageType);
        return !authenticationRequired
               || (_isAuthenticated && HasRoles(roles));
    }

    private (bool authenticationRequired, IEnumerable<string> roles) GetAuthenticationData(Type pageType)
    {
        if (!_routeProvider.TryGetRouteData(pageType, out var route)) return (false, Array.Empty<string>());

        var requiredRoles = new List<string>();
        var authenticationRequired = false;

        do
        {
            if (route.TryGetMetadata<AuthenticationMetadata>(out var authenticationMetadata))
            {
                authenticationRequired |= authenticationMetadata.AuthenticationRequired;
                requiredRoles.AddIfNotNullOrEmpty(authenticationMetadata.RoleName);

                if (authenticationMetadata.IgnoreParents)
                {
                    break;
                }
            }

            route = route.Parent;
        } while (route is not null);

        authenticationRequired |= requiredRoles.Any();

        return (authenticationRequired, requiredRoles);
    }

    public bool HasRoles(IEnumerable<string> requiredRoles)
    {
        return requiredRoles.All(_roles.Contains);
    }
}