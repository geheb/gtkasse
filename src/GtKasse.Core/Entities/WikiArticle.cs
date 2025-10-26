namespace GtKasse.Core.Entities;

using GtKasse.Core.Converter;
using GtKasse.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("wiki_articles")]
public sealed class WikiArticle : IEntity, IDtoMapper<WikiArticleDto>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    [Required]
    public DateTimeOffset Created { get; set; }

    public DateTimeOffset? Updated { get; set; }

    public Guid? UserId { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public IdentityUserGuid? User { get; set; }

    [Required]
    [MaxLength(16)]
    public string? Identifier { get; set; }

    [Required]
    [MaxLength(256)]
    public string? Title { get; set; }

    [Required]
    public string? Content { get; set; }

    public void FromDto(WikiArticleDto model)
    {
        Id = model.Id;
        UserId = model.UserId;
        Identifier = model.Identifier;
        Title = model.Title;
        Content = model.Content;
    }

    public WikiArticleDto ToDto(GermanDateTimeConverter dc)
    {
        return new()
        {
            Id = Id,
            LastUpdate = dc.ToLocal(Updated ?? Created),
            UserId = UserId,
            User = User?.Name,
            UserEmail = User?.EmailConfirmed == true ? User.Email : null,
            Identifier = Identifier,
            Title = Title,
            Content = Content
        };
    }
}
