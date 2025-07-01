using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKasse.Ui.Pages.Users;

[Node("Benutzer", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,usermanager")]
public class IndexModel : PageModel
{
    private readonly Core.Repositories.IdentityRepository _identityRepository;

    public IdentityDto[] Users { get; private set; } = [];
    public int UsersConfirmed { get; set; }
    public int UsersNotConfirmed { get; set; }
    public int UsersLocked { get; set; }

    public IndexModel(Core.Repositories.IdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task OnGetAsync(int filter, CancellationToken cancellationToken)
    {
        var users = await _identityRepository.GetAll(cancellationToken);
        if (filter == 1)
        {
            Users = [.. users.Where(u => u.Roles!.Contains(Roles.Interested))];
        }
        else
        {
            Users = users;
        }

        UsersConfirmed = Users.Count(u => u.IsEmailConfirmed);
        UsersNotConfirmed = Users.Count(u => !u.IsEmailConfirmed);
        UsersLocked = Users.Count(u => u.IsLocked);
    }
}
