using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKasse.Ui.Pages.Login;

[Authorize]
public class ExitModel : PageModel
{
    private readonly Core.Repositories.Users _users;

    public ExitModel(Core.Repositories.Users users)
    {
        _users = users;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await _users.SignOutCurrentUser();
        return Redirect("/");
    }
}
