using GtKasse.Core.Converter;
using GtKasse.Core.Database;
using GtKasse.Core.Entities;
using GtKasse.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GtKasse.Core.Repositories;

public sealed class FoodBookings
{
    private readonly UuidPkGenerator _pkGenerator = new();
    private readonly AppDbContext _dbContext;

    public FoodBookings(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BookingFoodDto[]> GetUntilEndOfMonth(DateTime start, CancellationToken cancellationToken)
    {
        var startParam = new DateTimeOffset(start, TimeSpan.Zero);
        var endParam = new DateTimeOffset(start.Year, start.Month, DateTime.DaysInMonth(start.Year, start.Month), 23, 59, 59, 999, TimeSpan.Zero);

        var dbSet = _dbContext.Set<FoodBooking>();

        var entities = await dbSet
            .AsNoTracking()
            .Include(e => e.Food)
            .Include(e => e.User)
            .Include(e => e.Invoice)
            .Where(e => e.BookedOn >= startParam && e.BookedOn <= endParam)
            .OrderByDescending(e => e.BookedOn)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();

        return entities.Select(e => new BookingFoodDto(e, dc)).ToArray();
    }

    public async Task<BookingFoodDto[]> GetForOneMonth(Guid userId, int year, int month, CancellationToken cancellationToken)
    {
        var start = new DateTimeOffset(year, month, 1, 0, 0, 0, TimeSpan.Zero);
        var end = new DateTimeOffset(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59, TimeSpan.Zero);
        var dbSet = _dbContext.Set<FoodBooking>();

        var entities = await dbSet
            .AsNoTracking()
            .Include(e => e.Food)
            .Include(e => e.Invoice)
            .Where(e => e.UserId == userId && e.BookedOn >= start && e.BookedOn <= end)
            .OrderByDescending(e => e.BookedOn)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();

        return entities.Select(e => new BookingFoodDto(e, dc)).ToArray();
    }

    public async Task<decimal> CalcTotal(DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        var start = new DateTimeOffset(from, TimeSpan.Zero);
        var end = new DateTimeOffset(to, TimeSpan.Zero);

        var statusCompleted = (int)BookingStatus.Completed;
        var dbSet = _dbContext.Set<FoodBooking>();

        var sumPrice = await dbSet
            .AsNoTracking()
            .Where(e => e.BookedOn >= start && e.BookedOn <= end && e.Status == statusCompleted && e.InvoiceId == null)
            .SumAsync(e => e.Food!.Price * e.Count, cancellationToken);

        return sumPrice;
    }

    public async Task<BookingFoodDto[]> GetNotCancelledForOneDay(DateTime date, CancellationToken cancellationToken)
    {
        var start = new DateTimeOffset(date.Date, TimeSpan.Zero);
        var end = new DateTimeOffset(date.Year, date.Month, date.Day, 23, 59, 59, TimeSpan.Zero);

        var statusCancelled = (int)BookingStatus.Cancelled;
        var dbSet = _dbContext.Set<FoodBooking>();

        var entities = await dbSet
            .AsNoTracking()
            .Include(e => e.Food)
            .Include(e => e.User)
            .Where(e => e.BookedOn >= start && e.BookedOn <= end && e.Status != statusCancelled && e.Food!.Type != (int)FoodType.Donation)
            .OrderByDescending(e => e.BookedOn)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();

        return entities.Select(e => new BookingFoodDto(e, dc)).ToArray();
    }

    public async Task<BookingFoodDto[]> GetInvoiceBookings(Guid invoiceId, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<FoodBooking>();

        var entities = await dbSet
            .AsNoTracking()
            .Include(e => e.Food)
            .Where(e => e.InvoiceId == invoiceId)
            .OrderBy(e => e.BookedOn)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();

        return entities.Select(e => new BookingFoodDto(e, dc)).ToArray();
    }

    public async Task<bool> Cancel(Guid userId, Guid bookingId, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<FoodBooking>();

        var entity = await dbSet
            .Include(e => e.User)
            .Include(e => e.Food)
            .FirstOrDefaultAsync(e => e.Id == bookingId, cancellationToken);

        if (entity == null) return false;
        if (entity.UserId != userId) return false;
        if (entity.Food?.Type != (int)FoodType.Donation && entity.Status != (int)BookingStatus.Confirmed) return false;
        if (entity.Food?.Type == (int)FoodType.Donation && entity.Status != (int)BookingStatus.Completed) return false;

        if (entity.InvoiceId.HasValue) return false;

        entity.Status = (int)BookingStatus.Cancelled;
        entity.CancelledOn = DateTimeOffset.UtcNow;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> Complete(Guid bookingId, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<FoodBooking>();

        var entity = await dbSet.FindAsync([bookingId], cancellationToken);

        if (entity == null) return false;
        if (entity.Status != (int)BookingStatus.Confirmed) return false;
        if (entity.InvoiceId.HasValue) return false;

        entity.Status = (int)BookingStatus.Completed;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> Create(Guid userId, Guid? foodId, int count, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<Food>();

        var food = await dbSet.FindAsync(foodId);
        if (food == null) return false;

        var entity = new FoodBooking
        {
            Id = _pkGenerator.Generate(),
            UserId = userId,
            FoodId = foodId,
            Status = (FoodType)food.Type == FoodType.Donation ? (int)BookingStatus.Completed : (int)BookingStatus.Confirmed,
            Count = count,
            BookedOn = DateTimeOffset.UtcNow
        };

        var dbSetBooking = _dbContext.Set<FoodBooking>();

        await dbSetBooking.AddAsync(entity, cancellationToken);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
