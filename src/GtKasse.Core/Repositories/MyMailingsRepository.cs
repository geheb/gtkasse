using FluentResults;
using GtKasse.Core.Converter;
using GtKasse.Core.Entities;
using GtKasse.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GtKasse.Core.Repositories;

public sealed class MyMailingsRepository : Repository<MyMailing, MyMailingDto>
{
    protected override IQueryable<MyMailing> GetBaseQuery() => _dbSet.AsNoTracking().Include(e => e.Mailing);

    public MyMailingsRepository(TimeProvider timeProvider, DbSet<MyMailing> dbSet)
        : base(timeProvider, dbSet)
    {
    }

    public async Task<MyMailingDto[]> GetByUser(Guid userId, CancellationToken cancellationToken)
    {
        var entities = await GetBaseQuery()
            .Where(x => x.UserId == userId)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();
        return [.. entities.Select(e => e.ToDto(dc))];
    }

    public async Task<Result> UpdateHasRead(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return NotFound;
        }

        entity.Updated = _timeProvider.GetUtcNow();
        entity.HasRead = true;

        return Result.Ok();
    }
}