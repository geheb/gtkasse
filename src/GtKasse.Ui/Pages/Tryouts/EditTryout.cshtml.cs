using GtKasse.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GtKasse.Ui.Pages.Tryouts;

[Node("Training bearbeiten", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,tripmanager")]
public class EditTryoutModel : PageModel
{
    private readonly Core.Repositories.Users _users;
    private readonly Core.Repositories.Tryouts _tryouts;

    [BindProperty]
    public TryoutInput Input { get; set; } = new();

    public string? Details { get; private set; }

    public SelectListItem[] Users { get; set; } = Array.Empty<SelectListItem>();

    public bool IsDisabled { get; set; }

    public EditTryoutModel(Core.Repositories.Users users, Core.Repositories.Tryouts tryouts)
    {
        _users = users;
        _tryouts = tryouts;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var tryout = await UpdateView(id, cancellationToken);
        if (tryout == null) return;
        Input.From(tryout);
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        if (await UpdateView(id, cancellationToken) == null) return Page();

        var error = Input.Validate();
        if (!string.IsNullOrEmpty(error))
        {
            ModelState.AddModelError(string.Empty, error);
            return Page();
        }

        var dto = Input.ToDto();
        dto.Id = id;

        var result = await _tryouts.UpdateTryout(dto, cancellationToken);
        if (!result)
        {
            ModelState.AddModelError(string.Empty, "Fehler beim Anlegen des Trainings.");
            return Page();
        }

        return RedirectToPage("Index");
    }

    public async Task<IActionResult> OnPostDeleteTryoutAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _tryouts.DeleteTryout(id, cancellationToken);
        return new JsonResult(result);
    }

    private async Task<TryoutDto?> UpdateView(Guid id, CancellationToken cancellationToken)
    {
        var tryout = await _tryouts.FindTryout(id, cancellationToken);

        if (tryout == null)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, "Fehler beim Laden des Trainings.");
            return null;
        }

        Details = new GermanDateTimeConverter().ToDateTime(tryout.Date);

        var users = await _users.GetAll(cancellationToken);

        var contactId = Guid.TryParse(Input.UserId, out var cid) ? cid : User.GetId();

        var items = new List<SelectListItem> { new() };
        items.AddRange(users
            .Where(u => u.Roles!.Any(r => r == Roles.TripManager))
            .Select(u => new SelectListItem(u.Name, u.Id.ToString(), u.Id == contactId)));
        Users = items.ToArray();

        return ModelState.IsValid ? tryout : default;
    }
}
