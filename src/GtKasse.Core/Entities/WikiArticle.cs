namespace GtKasse.Core.Entities;

using System;

public sealed class WikiArticle
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public Guid? UserId { get; set; }
    public IdentityUserGuid? User { get; set; }
    public string? Identifier { get; set; }
    public string? Title { get; set; }
    public string? DescriptionMember { get; set; }
    public string? DescriptionManagementBoard { get; set; }
}
