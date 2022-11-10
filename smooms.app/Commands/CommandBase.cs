using MediatR;

namespace smooms.app.Commands;

public abstract record CommandBase<TResponse> : IRequest<TResponse>;
public abstract record CommandBase : CommandBase<Unit>;

public abstract class CommandHanderBase<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> 
    where TRequest : CommandBase<TResponse>
{
    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
}

public abstract class CommandHanderBase<TRequest> : CommandHanderBase<TRequest, Unit> 
    where TRequest : CommandBase
{
    public sealed override async Task<Unit> Handle(TRequest request, CancellationToken cancellationToken = default)
    {
        await HandleUnit(request, cancellationToken);
        return Unit.Value;
    }

    public abstract Task HandleUnit(TRequest request, CancellationToken cancellationToken = default);
}