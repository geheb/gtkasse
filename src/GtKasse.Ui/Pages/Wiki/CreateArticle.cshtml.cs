namespace GtKasse.Ui.Pages.Wiki;

using GtKasse.Core.Repositories;
using GtKasse.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

[Node("Artikel anlegen", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,chairperson")]
public class CreateArticleModel : PageModel
{
    private readonly WikiArticles _wikiArticles;
    private readonly Users _users;

    [BindProperty]
    public WIkiArticleInput Input { get; set; } = new WIkiArticleInput();
    public SelectListItem[] Users { get; set; } = Array.Empty<SelectListItem>();

    public CreateArticleModel(WikiArticles wikiArticles, Users users)
    {
        _wikiArticles = wikiArticles;
        _users = users;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await UpdateView(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!await UpdateView(cancellationToken)) return Page();

        if (Input.IsDescriptionEmpty)
        {
            ModelState.AddModelError(string.Empty, "Eine Beschreibung wird benötigt.");
            return Page();
        }

        if (await _wikiArticles.HasIdentifier(Input.Identifier!, cancellationToken))
        {
            ModelState.AddModelError(string.Empty, "Die Kennung existiert bereits.");
            return Page();
        }

        WikiArticleDto dto = Input;

        var result = await _wikiArticles.Create(dto, cancellationToken);
        if (!result)
        {
            ModelState.AddModelError(string.Empty, "Fehler beim Erstellen des Artikels.");
            return Page();
        }

        return RedirectToPage("Index");
    }

    private async Task<bool> UpdateView(CancellationToken cancellationToken)
    {
        var users = await _users.GetAll(cancellationToken);

        var contactId = Guid.TryParse(Input.UserId, out var id) ? id : User.GetId();

        var items = new List<SelectListItem> { new() };

        items.AddRange(users
            .Where(u => u.Roles!.Any(r => r == Roles.Member))
            .Select(u => new SelectListItem(u.Name, u.Id.ToString(), u.Id == contactId)));

        Users = items.ToArray();

        return ModelState.IsValid;
    }
}
