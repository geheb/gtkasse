namespace GtKasse.Ui.Pages.Tryouts;

using GtKasse.Core.Repositories;
using GtKasse.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;


[Node("Training anlegen", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,tripmanager")]
public class CreateTryoutModel : PageModel
{
    private readonly IdentityRepository _identityRepository;
    private readonly Tryouts _tryouts;

    [BindProperty]
    public TryoutInput Input { get; set; } = new();

    public SelectListItem[] Users { get; private set; } = Array.Empty<SelectListItem>();

    public CreateTryoutModel(
        IdentityRepository identityRepository, 
        Tryouts tryouts)
    {
        _identityRepository = identityRepository;
        _tryouts = tryouts;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await UpdateView(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!await UpdateView(cancellationToken)) return Page();

        var error = Input.Validate();
        if (!string.IsNullOrEmpty(error))
        {
            ModelState.AddModelError(string.Empty, error);
            return Page();
        }
        var dto = Input.ToDto();

        var result = await _tryouts.CreateTryout(dto, cancellationToken);
        if (!result)
        {
            ModelState.AddModelError(string.Empty, "Fehler beim Anlegen des Trainings.");
            return Page();
        }

        return RedirectToPage("Index");
    }

    private async Task<bool> UpdateView(CancellationToken cancellationToken)
    {
        var users = await _identityRepository.GetAll(cancellationToken);

        var contactId = Guid.TryParse(Input.UserId, out var id) ? id : User.GetId();

        var items = new List<SelectListItem> { new() };
        items.AddRange(users
            .Where(u => u.Roles!.Any(r => r == Roles.TripManager))
            .Select(u => new SelectListItem(u.Name, u.Id.ToString(), u.Id == contactId)));
        Users = items.ToArray();

        return ModelState.IsValid;
    }
}
