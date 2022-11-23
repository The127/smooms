using smooms.api.Models;

namespace smooms.api.Services;

public interface IAuthenticationService
{
    void Login(SessionId sessionId, HttpResponse response);
    void Logout(HttpResponse response);
    SessionId? GetSessionId(HttpRequest request);
}

public class AuthenticationService : IAuthenticationService
{
    private const string CookieName = "smooms-session-id";
    
    public void Login(SessionId sessionId, HttpResponse response)
    {
        response.Cookies.Append(CookieName, sessionId.Value.ToString());
    }

    public void Logout(HttpResponse response)
    {
        response.Cookies.Delete(CookieName);
    }
    
    public SessionId? GetSessionId(HttpRequest request)
    {
        var cookie = request.Cookies[CookieName];
        if (cookie == null)
            return null;
        
        return new SessionId
        {
            Value = Guid.Parse(cookie),
        };
    }
}