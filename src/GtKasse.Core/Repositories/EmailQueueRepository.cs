using GtKasse.Core.Database;
using GtKasse.Core.Entities;
using GtKasse.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GtKasse.Core.Repositories;

public sealed class EmailQueueRepository
{
    private readonly UuidPkGenerator _pkGenerator = new();
    private readonly TimeProvider _timeProvider;
    private readonly AppDbContext _dbContext;

    public EmailQueueRepository(
        TimeProvider timeProvider,
        AppDbContext dbContext)
    {
        _timeProvider = timeProvider;
        _dbContext = dbContext;
    }

    public async Task<bool> Create(EmailQueueDto dto, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<EmailQueue>();
        await dbSet.AddAsync(new()
        {
            Id = _pkGenerator.Generate(),
            Created = _timeProvider.GetUtcNow(),
            Subject = dto.Subject,
            Recipient = dto.Recipient,
            HtmlBody = dto.HtmlBody,
            IsPrio = dto.IsPrio,
        }, cancellationToken);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<EmailQueueDto[]> GetNextToSend(int count, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<EmailQueue>();
        var entities = await dbSet
            .AsNoTracking()
            .Where(e => e.Sent == null)
            .OrderBy(e => e.Created).ThenByDescending(e => e.IsPrio)
            .Select(e => new { e.Id, e.Subject, e.Recipient, e.HtmlBody })
            .Take(count)
            .ToArrayAsync(cancellationToken);

        var result = new List<EmailQueueDto>();

        foreach (var e in entities)
        {
            result.Add(new() { Id = e.Id, Subject = e.Subject, Recipient = e.Recipient, HtmlBody = e.HtmlBody });
        }

        return result.ToArray();
    }

    public async Task<bool> UpdateSent(Guid id, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<EmailQueue>();

        var entity = await dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        entity.Sent = _timeProvider.GetUtcNow();

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
