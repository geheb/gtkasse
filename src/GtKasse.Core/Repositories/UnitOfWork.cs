using GtKasse.Core.Database;
using GtKasse.Core.Entities;
using GtKasse.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace GtKasse.Core.Repositories;

public sealed class UnitOfWork
{
    private readonly TimeProvider _timeProvider;
    private readonly AppDbContext _dbContext;
    private MailingRepository? _mailings;
    private EmailQueueRepository? _emailQueue;

    public MailingRepository Mailings =>
        _mailings ??= new(_timeProvider, _dbContext.Set<Mailing>());

    public EmailQueueRepository EmailQueue =>
        _emailQueue ??= new(_timeProvider, _dbContext.Set<EmailQueue>());

    public UnitOfWork(
        TimeProvider timeProvider,
        AppDbContext dbContext)
    {
        _timeProvider = timeProvider;
        _dbContext = dbContext;
    }

    public Task<IDbContextTransaction> BeginTran(CancellationToken cancellationToken) => _dbContext.Database.BeginTransactionAsync(cancellationToken);

    public Task<int> Save(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}
