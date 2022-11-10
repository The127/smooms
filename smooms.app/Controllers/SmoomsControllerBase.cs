using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace smooms.app.Controllers;

public class SmoomsControllerBase : ControllerBase
{
    protected readonly IMediator Mediator;

    public SmoomsControllerBase(IMediator mediator)
    {
        Mediator = mediator;
    }
}