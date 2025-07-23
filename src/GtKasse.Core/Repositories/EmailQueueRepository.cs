using FluentResults;
using GtKasse.Core.Entities;
using GtKasse.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GtKasse.Core.Repositories;

public sealed class EmailQueueRepository : Repository<EmailQueue, EmailQueueDto>
{
    public EmailQueueRepository(TimeProvider timeProvider, DbSet<EmailQueue> dbSet) 
        : base(timeProvider, dbSet)
    {
    }

    public async Task<EmailQueueDto[]> GetNextToSend(int count, CancellationToken cancellationToken)
    {
        var entities = await _dbSet
            .AsNoTracking()
            .Where(e => e.Sent == null)
            .OrderBy(e => e.Created).ThenByDescending(e => e.IsPrio)
            .Select(e => new { e.Id, e.Subject, e.Recipient, e.HtmlBody, e.ReplyAddress })
            .Take(count)
            .ToArrayAsync(cancellationToken);

        var result = new List<EmailQueueDto>();

        foreach (var e in entities)
        {
            result.Add(new() 
            { 
                Id = e.Id, 
                Subject = e.Subject, 
                Recipient = e.Recipient, 
                HtmlBody = e.HtmlBody,
                ReplyAddress = e.ReplyAddress
            });
        }

        return result.ToArray();
    }

    public async Task<Result> UpdateSent(Guid[] ids, CancellationToken cancellationToken)
    {
        var entities = await _dbSet
            .Where(e => ids.Contains(e.Id))
            .ToArrayAsync(cancellationToken);

        if (entities.Length == 0)
        {
            return Result.Fail("Keine Datensätze gefunden.");
        }

        var now = _timeProvider.GetUtcNow();
        foreach (var e in entities)
        { 
            e.Sent = now;
        }

        return Result.Ok();
    }
}
