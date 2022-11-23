using MediatR;
using Microsoft.AspNetCore.Mvc;
using smooms.api.Commands;
using smooms.api.Services;

namespace smooms.api.Controllers;

[Route("api/sessions")]
public class SessionController : SmoomsControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    
    public SessionController(IMediator mediator, IAuthenticationService authenticationService) : base(mediator)
    {
        _authenticationService = authenticationService;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateSession(CreateSessionRequestDto request)
    {
        var command = new CreateSessionCommand(request.Email, request.Password);
        var result = await Mediator.Send(command);
        _authenticationService.Login(result.SessionId, HttpContext.Response);
        return Ok();
    }
    
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteSession()
    {
        var sessionId = _authenticationService.GetSessionId(HttpContext.Request);
        if (sessionId is not null)
        {
            var command = new DeleteSessionCommand(sessionId);
            await Mediator.Send(command);
            _authenticationService.Logout(HttpContext.Response);            
        }
        return Ok();
    }
}