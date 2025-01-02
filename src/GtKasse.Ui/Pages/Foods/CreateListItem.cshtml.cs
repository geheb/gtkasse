using GtKasse.Ui.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GtKasse.Ui.Pages.Foods;

[Node("Eintrag anlegen", FromPage = typeof(EditListItemsModel))]
[Authorize(Roles = "administrator,treasurer")]
public class CreateListItemModel : PageModel
{
    private readonly Core.Repositories.Foods _foods;

    [Display(Name = "Typ")]
    [BindProperty, RequiredField]
    public int Type { get; set; } = (int)FoodType.Drink;

    [Display(Name = "Name")]
    [BindProperty, RequiredField, TextLengthField]
    public string? Name { get; set; }

    public const double MinPrice = 0.1;
    public const double MaxPrice = 10.0;

    [Display(Name = "Preis")]
    [BindProperty, RequiredField]
    [DataType(DataType.Currency)]
    [RangeField(MinPrice, MaxPrice)]
    public decimal Price { get; set; } = 1m;

    public string? ListDetails { get; set; } = "n.v.";
    public bool IsDisabled { get; set; }

    public CreateListItemModel(Core.Repositories.Foods foods)
    {
        _foods = foods;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        await UpdateListDetails(id, cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!await UpdateListDetails(id, cancellationToken)) return Page();

        if (!ModelState.IsValid) return Page();

        var dto = new FoodDto();
        dto.Type = (FoodType)Type;
        dto.Name = Name;
        dto.Price = Price;
        dto.FoodListId = id;

        if (!await _foods.Create(dto, cancellationToken))
        {
            ModelState.AddModelError(string.Empty, "Fehler beim Anlegen des Eintrages.");
            return Page();
        }

        return RedirectToPage("EditListItems", new { id });
    }

    private async Task<bool> UpdateListDetails(Guid id, CancellationToken cancellationToken)
    {
        var foodList = await _foods.Find(id, cancellationToken);
        if (foodList == null)
        {
            IsDisabled = true;
            return false;
        }
        var datetimeConverter = new GermanDateTimeConverter();
        ListDetails = foodList.Name + ", g�ltig ab " + datetimeConverter.ToDateTime(foodList.ValidFrom);
        return true;
    }
}
