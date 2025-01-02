using GtKasse.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKasse.Ui.Pages.MyInvoices;

[Node("Rechnungsdetails", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,member")]
public class DetailsModel : PageModel
{
    private readonly Core.Repositories.Users _users;
    private readonly Core.Repositories.Invoices _invoices;
    private readonly Core.Repositories.Bookings _bookings;

    public string? Recipient { get; set; } = "n.v.";
    public string? Description { get; set; } = "n.v.";
    public string? Period { get; set; } = "n.v.";
    public decimal Total { get; set; }
    public BookingFoodDto[] Bookings { get; set; } = Array.Empty<BookingFoodDto>();

    public DetailsModel(
        Core.Repositories.Users users,
        Core.Repositories.Invoices invoices,
        Core.Repositories.Bookings bookings)
    {
        _users = users;
        _invoices = invoices;
        _bookings = bookings;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var invoice = await _invoices.Find(id, User.GetId(), cancellationToken);
        if (invoice == null) return;

        var user = await _users.Find(User.GetId(), cancellationToken);

        Recipient = user?.Name;
        Description = invoice.Description;
        Period = invoice.Period;
        Total = invoice.Total;

        Bookings = await _bookings.GetInvoiceBookings(id, cancellationToken);
    }
}
