using LazyCache;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using NodaTime;

namespace smooms.app.Behaviours;

public interface ICachableQuery<out TResponse> : IRequest<TResponse>
{
    string CacheKey { get; }
    Duration? CacheDuration { get; }
}

public class CachingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachableQuery<TResponse>
{
    private readonly IAppCache _appCache;

    public CachingBehaviour(IAppCache appCache)
    {
        _appCache = appCache;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var cacheKey = $"{typeof(TRequest).Name}@{request.CacheKey}";

        var response = await _appCache.GetOrAddAsync(
            cacheKey, 
            async entry =>
            {
                entry.SlidingExpiration = request.CacheDuration?.ToTimeSpan();
                entry.Priority = request.CacheDuration.HasValue ? CacheItemPriority.Normal : CacheItemPriority.NeverRemove;
                return await next();
            });

        return response;
    }
}