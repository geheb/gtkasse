using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GtKasse.Ui.Pages.Invoices
{
    [Node("Rechnungen", FromPage = typeof(Pages.IndexModel))]
    [Authorize(Roles = "administrator,treasurer")]
    public class IndexModel : PageModel
    {
        private readonly Core.Repositories.Invoices _invoices;

        public InvoiceDto[] Invoices { get; set; } = Array.Empty<InvoiceDto>();
        public SelectListItem[] FilterItems { get; set; } = Array.Empty<SelectListItem>();
        public decimal InvoicesTotal { get; set; }
        public decimal InvoicesOpenTotal { get; set; }
        public decimal InvoicesPaidTotal { get; set; }
        public string? FilterPeriodId { get; set; }

        public IndexModel(Core.Repositories.Invoices invoices)
        {
            _invoices = invoices;
        }

        public async Task OnGetAsync([StringLength(36)] string? filter = null, CancellationToken cancellationToken = default)
        {
            FilterItems = await CreateListItems(cancellationToken);

            var selectedItem = 
                Guid.TryParse(filter, out var filterPeriodId) ?
                FilterItems.FirstOrDefault(f => f.Value.Equals(filter, StringComparison.OrdinalIgnoreCase)) : 
                null;

            if (selectedItem == null && FilterItems.Length > 0) selectedItem = FilterItems[0];
            if (selectedItem == null) return;
            
            selectedItem.Selected = true;
            FilterPeriodId = selectedItem.Value;
            filterPeriodId = Guid.Parse(FilterPeriodId);

            Invoices = await _invoices.GetByPeriod(filterPeriodId, cancellationToken);

            InvoicesTotal = Invoices.Sum(i => i.Total);
            InvoicesOpenTotal = Invoices.Sum(i => i.OpenTotal);
            InvoicesPaidTotal = Invoices.Sum(i => i.PaidTotal);
        }

        public async Task<IActionResult> OnPostStatusPaidAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await _invoices.UpdateStatusPaid(id, cancellationToken);
            return new JsonResult(result);
        }

        public async Task<IActionResult> OnPostStatusPaidAllAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await _invoices.UpdateStatusPaidAll(id, cancellationToken);
            return new JsonResult(result);
        }

        public async Task<IActionResult> OnPostStatusOpenAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await _invoices.UpdateStatusOpen(id, cancellationToken);
            return new JsonResult(result);
        }

        private async Task<SelectListItem[]> CreateListItems(CancellationToken cancellationToken)
        {
            var periods = await _invoices.GetPeriods(cancellationToken);

            if (periods.Length < 1) return Array.Empty<SelectListItem>();

            var items = new List<SelectListItem>();

            foreach (var p in periods)
            {
                var fullName = p.Period + (string.IsNullOrEmpty(p.Description) ? string.Empty : $" ({p.Description})");
                items.Add(new SelectListItem(fullName, p.Id.ToString()));
            }

            return items.ToArray();
        }
    }
}
