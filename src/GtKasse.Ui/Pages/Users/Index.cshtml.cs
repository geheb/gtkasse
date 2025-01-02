using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKasse.Ui.Pages.Users;

[Node("Benutzer", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,usermanager")]
public class IndexModel : PageModel
{
    private readonly Core.Repositories.Users _users;
    public UserDto[] Users { get; private set; } = Array.Empty<UserDto>();
    public int UsersConfirmed { get; set; }
    public int UsersNotConfirmed { get; set; }
    public int UsersLocked { get; set; }

    public IndexModel(Core.Repositories.Users users)
    {
        _users = users;
    }

    public async Task OnGetAsync(int filter, CancellationToken cancellationToken)
    {
        var users = await _users.GetAll(cancellationToken);
        if (filter == 1)
        {
            Users = users.Where(u => u.Roles!.Contains(Roles.Interested)).ToArray();
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
