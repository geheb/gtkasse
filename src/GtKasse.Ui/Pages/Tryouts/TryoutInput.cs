﻿using GtKasse.Ui.Annotations;
using System.ComponentModel.DataAnnotations;

namespace GtKasse.Ui.Pages.Tryouts;

public sealed class TryoutInput
{
    [Display(Name = "Trainingsdatum")]
    [RequiredField]
    public string? Date { get; set; }

    [Display(Name = "Ansprechpartner")]
    [RequiredField]
    public string? UserId { get; set; }

    [Display(Name = "Anmeldungen maximal")]
    [RequiredField, RangeField(1, 100)]
    public int MaxBookings { get; set; } = 20;

    [Display(Name = "Anmeldungen von")]
    [RequiredField]
    public string? BookingStart { get; set; }

    [Display(Name = "Anmeldungen bis")]
    [RequiredField]
    public string? BookingEnd { get; set; }

    [Display(Name = "Beschreibung")]
    [TextLengthField(1000)]
    public string? Description { get; set; }

    public string? Validate()
    {
        var dc = new GermanDateTimeConverter();
        var startDate = dc.FromIsoDateTime(Date)!.Value;

        var regStartDate = dc.FromIsoDateTime(BookingStart)!.Value;
        var regEndDate = dc.FromIsoDateTime(BookingEnd)!.Value;

        if (regStartDate >= regEndDate || regStartDate >= startDate || regEndDate >= startDate)
        {
            return "Die Anmeldung sollte vor dem Training stattfinden.";
        }

        return null;
    }

    internal TryoutDto ToDto()
    {
        var dc = new GermanDateTimeConverter();
        return new()
        {
            Date = dc.FromIsoDateTime(Date)!.Value,
            UserId = Guid.Parse(UserId!),
            MaxBookings = MaxBookings,
            BookingStart = dc.FromIsoDateTime(BookingStart)!.Value,
            BookingEnd = dc.FromIsoDateTime(BookingEnd)!.Value,
            Description = Description
        };
    }

    internal void From(TryoutDto dto)
    {
        var dc = new GermanDateTimeConverter();
        Date = dc.ToIso(dto.Date);
        UserId = dto.UserId.ToString();
        MaxBookings = dto.MaxBookings;
        BookingStart = dc.ToIso(dto.BookingStart);
        BookingEnd = dc.ToIso(dto.BookingEnd);
        Description = dto.Description;
    }
}