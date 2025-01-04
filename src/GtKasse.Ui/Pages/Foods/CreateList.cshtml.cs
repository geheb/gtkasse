using GtKasse.Ui.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GtKasse.Ui.Pages.Foods;

[Node("Buchungsliste anlegen", FromPage = typeof(ListModel))]
[Authorize(Roles = "administrator,treasurer")]
public class CreateListModel : PageModel
{
    private readonly Core.Repositories.Foods _foods;

    [Display(Name = "Name")]
    [BindProperty, RequiredField, TextLengthField]
    public string? Name { get; set; } = $"Speisen, Getränke & Spenden {DateTime.UtcNow.Year}";

    [Display(Name = "Gültig ab")]
    [BindProperty, RequiredField]
    public string? ValidFrom { get; set; }

    public CreateListModel(Core.Repositories.Foods foods)
    {
        _foods = foods;
        var dc = new GermanDateTimeConverter();
        ValidFrom = dc.ToIso(dc.ToLocal(DateTimeOffset.UtcNow));
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();

        var dto = new FoodListDto();
        dto.Name = Name;
        dto.ValidFrom = new GermanDateTimeConverter().FromIsoDateTime(ValidFrom)!.Value;

        if (!await _foods.Create(dto, cancellationToken))
        {
            ModelState.AddModelError(string.Empty, "Fehler beim Anlegen der Buchungsliste.");
            return Page();
        }

        return RedirectToPage("EditListItems", new { id = dto.Id });
    }
}
