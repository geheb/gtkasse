using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKasse.Ui.Pages
{
    [AllowAnonymous]
    public class ImprintModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
