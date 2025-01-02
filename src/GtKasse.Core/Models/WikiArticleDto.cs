namespace GtKasse.Core.Models;

using GtKasse.Core.Converter;
using GtKasse.Core.Entities;
using System;

public sealed class WikiArticleDto
{
    public Guid Id { get; set; }
    public string? Identifier { get; set; }
    public string? Title { get; set; }
    public Guid? UserId { get; set; }
    public string? User { get; }
    public string? UserEmail { get; }
    public DateTimeOffset LastUpdate { get; }
    public string? DescriptionMember { get; set; }
    public string? DescriptionManagementBoard { get; set; }

    public WikiArticleDto()
    {
    }

    public WikiArticleDto(WikiArticle entity, GermanDateTimeConverter dc)
    {
        Id = entity.Id;
        UserId = entity.UserId;
        User = entity.User?.Name;
        UserEmail = entity.User?.EmailConfirmed == true ? entity.User?.Email : null;
        LastUpdate = entity.UpdatedOn is null ? dc.ToLocal(entity.CreatedOn) : dc.ToLocal(entity.UpdatedOn.Value);
        Identifier = entity.Identifier;
        Title = entity.Title;
        DescriptionMember = entity.DescriptionMember;
        DescriptionManagementBoard = entity.DescriptionManagementBoard;
    }
}
