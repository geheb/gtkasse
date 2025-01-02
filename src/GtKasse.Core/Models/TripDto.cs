﻿using GtKasse.Core.Converter;
using GtKasse.Core.Entities;

namespace GtKasse.Core.Models;

public sealed class TripDto
{
    public Guid Id { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public string? Target { get; set; }
    public Guid UserId { get; set; }
    public int MaxBookings { get; set; }
    public DateTimeOffset BookingStart { get; set; }
    public DateTimeOffset BookingEnd { get; set; }
    public string? Description { get; set; }

    internal TripDto(Trip entity, GermanDateTimeConverter dc)
    {
        Id = entity.Id;
        Start = dc.ToLocal(entity.Start);
        End = dc.ToLocal(entity.End);
        Target = entity.Target;
        UserId = entity.UserId!.Value;
        MaxBookings = entity.MaxBookings;
        BookingStart = dc.ToLocal(entity.BookingStart!.Value);
        BookingEnd = dc.ToLocal(entity.BookingEnd!.Value);
        Description = entity.Description;
    }

    public TripDto()
    {
    }

    internal Trip ToEntity()
    {
        return new()
        {
            Start = Start,
            End = End,
            Target = Target,
            UserId = UserId,
            MaxBookings = MaxBookings,
            BookingStart = BookingStart,
            BookingEnd = BookingEnd,
            Description = Description
        };
    }
}
