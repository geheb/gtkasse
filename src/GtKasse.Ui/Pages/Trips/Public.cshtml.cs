using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKasse.Ui.Pages.Trips;

[AllowAnonymous]
public class PublicModel : PageModel
{
    private readonly Core.Repositories.Trips _trips;

    public PublicTripDto[] Items { get; set; } = [];

    public PublicModel(Core.Repositories.Trips trips)
    {
        _trips = trips;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Items = await _trips.GetPublicTrips(cancellationToken);
    }
}
