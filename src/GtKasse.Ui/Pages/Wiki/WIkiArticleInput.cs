using GtKasse.Ui.Annotations;
using System.ComponentModel.DataAnnotations;

namespace GtKasse.Ui.Pages.Wiki;

public class WIkiArticleInput
{
    [Display(Name = "Kennung")]
    [RequiredField, TextLengthField(8, MinimumLength = 1)]
    public string? Identifier { get; set; }

    [Display(Name = "Titel")]
    [RequiredField, TextLengthField]
    public string? Title { get; set; }

    [Display(Name = "Ansprechpartner")]
    [RequiredField]
    public string? UserId { get; set; }

    [Display(Name = "Beschreibung (Mitglieder)")]
    [TextLengthField(8000)]
    public string? DescriptionMember { get; set; }

    [Display(Name = "Beschreibung (Vorstand)")]
    [TextLengthField(8000)]
    public string? DescriptionManagementBoard { get; set; }

    public bool IsDescriptionEmpty =>
        string.IsNullOrWhiteSpace(DescriptionMember) &&
        string.IsNullOrWhiteSpace(DescriptionManagementBoard);

    public static implicit operator WikiArticleDto(WIkiArticleInput input)
    {

        return new()
        {
            UserId = Guid.Parse(input.UserId!),
            Identifier = input.Identifier,
            Title = input.Title,
            DescriptionMember = input.DescriptionMember,
            DescriptionManagementBoard = input.DescriptionManagementBoard
        };
    }

    public static implicit operator WIkiArticleInput(WikiArticleDto dto)
    {
        return new()
        {
            Identifier = dto.Identifier,
            Title = dto.Title,
            UserId = dto.UserId?.ToString(),
            DescriptionMember = dto.DescriptionMember,
            DescriptionManagementBoard = dto.DescriptionManagementBoard
        };
    }
}
