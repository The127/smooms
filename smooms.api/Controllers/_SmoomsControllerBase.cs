using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace smooms.api.Controllers;

[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
public class SmoomsControllerBase : ControllerBase
{
    protected readonly IMediator Mediator;

    public SmoomsControllerBase(IMediator mediator)
    {
        Mediator = mediator;
    }
}