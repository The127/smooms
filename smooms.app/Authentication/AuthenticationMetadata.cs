using smooms.app.Utils;

namespace smooms.app.Authentication;

public class AuthenticationMetadata
{
    public required bool IgnoreParents { get; init; }
    
    private readonly bool _authenticationRequired;

    public required bool AuthenticationRequired
    {
        get => _authenticationRequired || !RoleName.IsNullOrWhiteSpace();
        init => _authenticationRequired = value;
    }

    public string? RoleName { get; init; }
}

public static class AuthenticationMetadataExtensions
{
    public static void RequireAuthentication(this FluentBlazorRouter.RouteGroupBuilder builder,
        bool ignoreParents = false)
    {
        builder.WithMetadata(new AuthenticationMetadata
        {
            AuthenticationRequired = true,
            IgnoreParents = ignoreParents,
        });
    }
    
    public static RouteGroupBuilder RequireRole(this RouteGroupBuilder builder, string roleName, bool ignoreParents = false)
    {
        builder.WithMetadata(new AuthenticationMetadata
        {
            RoleName = roleName,
            AuthenticationRequired = true,
            IgnoreParents = ignoreParents,
        });
        return builder;
    }
    
    public static RouteGroupBuilder AllowAnonymous(this RouteGroupBuilder builder, bool ignoreParents = false)
    {
        builder.WithMetadata(new AuthenticationMetadata
        {
            AuthenticationRequired = false,
            IgnoreParents = ignoreParents,
        });
        return builder;
    }
}