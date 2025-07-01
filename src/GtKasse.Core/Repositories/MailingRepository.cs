using FluentResults;
using GtKasse.Core.Entities;
using GtKasse.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GtKasse.Core.Repositories;

public sealed class MailingRepository : Repository<Mailing, MailingDto>
{
    public MailingRepository(TimeProvider timeProvider, DbSet<Mailing> dbSet) 
        : base(timeProvider, dbSet)
    {
    }

    public async Task<Result> UpdateClosed(Guid id, int emailCount, CancellationToken cancellationToken)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return NotFound;
        }

        entity.Updated = _timeProvider.GetUtcNow();
        entity.IsClosed = true;
        entity.EmailCount = emailCount;

        return Result.Ok();
    }
}
