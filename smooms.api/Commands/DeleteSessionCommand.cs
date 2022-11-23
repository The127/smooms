using Microsoft.EntityFrameworkCore;
using smooms.api.Models;

namespace smooms.api.Commands;

public record DeleteSessionCommand(SessionId SessionId) : CommandBase;

public class DeleteSessionCommandHandler : CommandHanderBase<DeleteSessionCommand>
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    public DeleteSessionCommandHandler(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public override async Task HandleUnit(DeleteSessionCommand request, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        await dbContext.Set<Session>()
            .Where(x => x.Id == request.SessionId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}