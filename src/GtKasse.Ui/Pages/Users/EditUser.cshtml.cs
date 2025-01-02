using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKasse.Ui.Pages.Users;

[Node("Benutzer bearbeiten", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,usermanager")]
public class EditUserModel : PageModel
{
    private readonly Core.Repositories.Users _users;

    [BindProperty]
    public EditUserInput Input { get; set; } = new EditUserInput();
    public bool IsDisabled { get; set; }
    public string? Name { get; set; }
    public bool CanBeDeleted { get; set; }

    public EditUserModel(Core.Repositories.Users users)
    {
        _users = users;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await UpdateView(id, cancellationToken);
        if (user == null) return;
        Input.From(user);
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await UpdateView(id, cancellationToken);
        if (user == null) return Page();

        if (Input.Roles.All(r => r == false))
        {
            ModelState.AddModelError(string.Empty, "Keine Rolle ausgewählt.");
            return Page();
        }
        
        Input.To(user);

        var errors = await _users.Update(user, Input.Password, cancellationToken);
        if (errors != null)
        {
            errors.ToList().ForEach(e => ModelState.AddModelError(string.Empty, e));
            return Page();
        }

        return RedirectToPage("Index");
    }

    private async Task<UserDto?> UpdateView(Guid id, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return default;

        var user = await _users.Find(id, cancellationToken);
        if (user == null)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, "Benutzer wurde nicht gefunden.");
            return default;
        }

        Name = user.Name ?? user.Email;
        CanBeDeleted = user.CanBeDeleted;
        return user;
    }

    public async Task<IActionResult> OnPostConfirmEmailAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _users.NotifyConfirmRegistration(id, false, cancellationToken);
        return new JsonResult(result);
    }

    public async Task<IActionResult> OnPostRemoveUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _users.RemoveUser(id, cancellationToken);
        return new JsonResult(result);
    }

    public async Task<IActionResult> OnPostDeleteUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _users.DeleteUser(id, cancellationToken);
        return new JsonResult(result);
    }

    public async Task<IActionResult> OnPostResetTwoFactorAsync(Guid id)
    {
        var result = await _users.ResetTwoFactor(id);
        return new JsonResult(result.IsSuccess);
    }
}
