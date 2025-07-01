using GtKasse.Core.User;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace GtKasse.Ui.Pages;

[Node("Startseite", IsDefault = true)]
[AllowAnonymous]
public class IndexModel : PageModel
{
}