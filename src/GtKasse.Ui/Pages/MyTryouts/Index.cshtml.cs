namespace GtKasse.Ui.Pages.MyTryouts;

using GtKasse.Core.Repositories;
using GtKasse.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Mein Anfängertraining", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,member,interested")]
public class IndexModel : PageModel
{
    private readonly Tryouts _tryouts;

    public MyTryoutListDto[] Items { get; set; } = Array.Empty<MyTryoutListDto>();

    public IndexModel(Tryouts tryouts) 
    {
        _tryouts = tryouts;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Items = await _tryouts.GetMyTryoutList(User.GetId(), cancellationToken);
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _tryouts.DeleteBooking(id, User.GetId(), cancellationToken);
        return new JsonResult(result);
    }
}
