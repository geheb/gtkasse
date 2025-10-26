using GtKasse.Core.Converter;
using GtKasse.Core.Database;
using GtKasse.Core.Entities;
using GtKasse.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace GtKasse.Core.Repositories;

public sealed class Invoices
{
    private readonly UuidPkGenerator _pkGenerator = new();
    private readonly AppDbContext _dbContext;

    public Invoices(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<InvoicePeriodDto[]> GetPeriods(CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<InvoicePeriod>();

        var entities = await dbSet
            .AsNoTracking()
            .OrderByDescending(e => e.To)
            .ToArrayAsync(cancellationToken);

        var ci = CultureInfo.GetCultureInfo("de-DE");

        return entities.Select(e => new InvoicePeriodDto(e, ci)).ToArray();
    }

    public async Task<InvoiceDto[]> GetByPeriod(Guid id, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<Invoice>();
        var entities = await dbSet
            .AsNoTracking()
            .Include(e => e.User)
            .Include(e => e.InvoicePeriod)
            .Where(e => e.InvoicePeriodId == id)
            .OrderBy(e => e.User!.DebtorNumber)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();
        var ci = CultureInfo.GetCultureInfo("de-DE");

        return entities.Select(e => new InvoiceDto(e, dc, ci)).ToArray();
    }

    public async Task<InvoiceDto[]> GetAll(Guid userId, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<Invoice>();
        var entities = await dbSet
            .AsNoTracking()
            .Include(e => e.InvoicePeriod)
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.CreatedOn)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();
        var ci = CultureInfo.GetCultureInfo("de-DE");

        return entities.Select(e => new InvoiceDto(e, dc, ci)).ToArray();
    }

    public async Task<InvoiceDto?> Find(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<Invoice>();
        var entity = await dbSet
            .AsNoTracking()
            .Include(e => e.InvoicePeriod)
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId, cancellationToken);

        if (entity == null) return null;

        var dc = new GermanDateTimeConverter();
        var ci = CultureInfo.GetCultureInfo("de-DE");

        return new InvoiceDto(entity, dc, ci);
    }

    public async Task<int> Create(DateTime start, DateTime end, string description, CancellationToken cancellationToken)
    {
        var statusCompleted = (int)BookingStatus.Completed;
        var startParam = new DateTimeOffset(start, TimeSpan.Zero);
        var endParam = new DateTimeOffset(end, TimeSpan.Zero);

        var dbSet = _dbContext.Set<FoodBooking>();

        var userIds = await dbSet
            .AsNoTracking()
            .Where(e => e.BookedOn >= startParam && e.BookedOn <= endParam && e.Status == statusCompleted && e.InvoiceId == null)
            .Select(e => e.UserId)
            .Distinct()
            .ToArrayAsync(cancellationToken);

        if (!userIds.Any()) return 0;

        var period = new InvoicePeriod
        {
            Id = _pkGenerator.Generate(),
            Description = description,
            From = startParam,
            To = endParam
        };

        await _dbContext.Set<InvoicePeriod>()
            .AddAsync(period, cancellationToken);

        var dbSetInvoice = _dbContext.Set<Invoice>();
        var count = 0;

        foreach (var user in userIds)
        {
            var bookings = await dbSet
                .Include(e => e.Food)
                .Where(e => e.UserId == user && e.BookedOn >= startParam && e.BookedOn <= endParam && e.Status == statusCompleted && e.InvoiceId == null)
                .ToArrayAsync(cancellationToken);

            if (!bookings.Any()) continue;

            var total = bookings.Sum(b => b.Food!.Price * b.Count);

            var invoice = new Invoice
            {
                Id = _pkGenerator.Generate(),
                CreatedOn = DateTimeOffset.UtcNow,
                Status = (int)InvoiceStatus.Open,
                UserId = user,
                Total = total,
                InvoicePeriodId = period.Id
            };

            await dbSetInvoice.AddAsync(invoice, cancellationToken);

            Array.ForEach(bookings, b => b.InvoiceId = invoice.Id);

            count++;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return count;
    }

    public async Task<bool> UpdateStatusPaid(Guid id, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<Invoice>();

        var entity = await dbSet.FindAsync([id], cancellationToken);
        if (entity == null) return false;
        var status = (InvoiceStatus)entity.Status;
        if (status == InvoiceStatus.Paid) return true;
        entity.Status = (int)InvoiceStatus.Paid;
        entity.PaidOn = DateTimeOffset.UtcNow;
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateStatusOpen(Guid id, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<Invoice>();

        var entity = await dbSet.FindAsync([id], cancellationToken);
        if (entity == null) return false;
        var status = (InvoiceStatus)entity.Status;
        if (status == InvoiceStatus.Open) return true;
        entity.Status = (int)InvoiceStatus.Open;
        entity.PaidOn = null;
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateStatusPaidAll(Guid periodId, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<Invoice>();

        const int statusOpen = (int)InvoiceStatus.Open;
        var entities = await dbSet
            .Where(e => e.InvoicePeriodId == periodId && e.Status == statusOpen)
            .ToArrayAsync(cancellationToken);

        if (entities.Length < 1) return false;

        foreach (var e in entities)
        {
            e.Status = (int)InvoiceStatus.Paid;
            e.PaidOn = DateTimeOffset.UtcNow;
        }

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
