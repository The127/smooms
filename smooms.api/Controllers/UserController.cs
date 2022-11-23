using HttpExceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using smooms.api.Commands;

namespace smooms.api.Controllers;

[Route("api/users")]
public class UserController : SmoomsControllerBase
{
    public UserController(IMediator mediator) : base(mediator)
    {
    }
    
    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateUser(CreateUserRequestDto dto, CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(dto.Name, dto.Email, dto.Password);
        await Mediator.Send(command, cancellationToken);
        return Ok();
    }
}