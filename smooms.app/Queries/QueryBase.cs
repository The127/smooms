using MediatR;

namespace smooms.app.Queries;

public record QueryBase<TResponse> : IRequest<TResponse>;

public record QueryBase : QueryBase<Unit>;

public abstract class QueryHandlerBase<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : QueryBase<TResponse>
{
    public abstract Task<TResponse> Handle(TQuery request, CancellationToken cancellationToken = default);
}

public abstract class QueryHandlerBase<TQuery> : QueryHandlerBase<TQuery, Unit> 
    where TQuery : QueryBase
{
    public sealed override async Task<Unit> Handle(TQuery request, CancellationToken cancellationToken = default)
    {
        await HandleUnit(request, cancellationToken);
        return Unit.Value;
    }
    
    public abstract Task HandleUnit(TQuery request, CancellationToken cancellationToken = default);
}