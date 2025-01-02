using GtKasse.Core.Repositories;
using GtKasse.Ui.Pages.Users;
using Microsoft.AspNetCore.Mvc;

namespace GtKasse.Ui.Controllers;

[ApiKey]
[ApiController]
[Route("/api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly Users _users;

    public AdminController(Users users)
    {
        _users = users;
    }

    [HttpPost("user")]
    public async Task<IActionResult> CreateUser([FromBody]CreateUserInput user, CancellationToken cancellationToken)
    {
        var dto = new UserDto();
        user.To(dto);

        var result = await _users.Create(dto, true, cancellationToken);

        return new JsonResult(result);
    }
}
