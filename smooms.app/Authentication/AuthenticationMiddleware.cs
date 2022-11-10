using FluentBlazorRouter;
using RouteData = Microsoft.AspNetCore.Components.RouteData;

namespace smooms.app.Authentication;

public class AuthenticationMiddleware : IRouterMiddleware
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationMiddleware(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public void Execute(Action next, RouteData pageContext)
    {
        if (_authenticationService.IsAuthorizedFor(pageContext.PageType))
        {
            next();
        }
    }
}