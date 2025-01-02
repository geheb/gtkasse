namespace GtKasse.Core.Models;

using GtKasse.Core.Converter;
using GtKasse.Core.Entities;
using System;

public sealed class WikiArticleListDto
{
    public Guid Id { get; set; }
    public string? User { get; set; }
    public string? UserEmail { get; set; }
    public DateTimeOffset LastUpdate { get; set; }
    public string? Identifier { get; set; }
    public string? Title { get; set; }

    public WikiArticleListDto(WikiArticle entity, GermanDateTimeConverter dc)
    {
        Id = entity.Id;
        User = entity.User?.Name;
        UserEmail = entity.User?.EmailConfirmed == true ? entity.User.Email : null;
        LastUpdate = entity.UpdatedOn is not null ? dc.ToLocal(entity.UpdatedOn!.Value) : dc.ToLocal(entity.CreatedOn);
        Identifier = entity.Identifier;
        Title = entity.Title;
    }
}
