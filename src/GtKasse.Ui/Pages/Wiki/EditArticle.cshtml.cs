namespace GtKasse.Ui.Pages.Wiki;

using GtKasse.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

[Node("Artikel bearbeiten", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,chairperson")]
public class EditArticleModel : PageModel
{
    private readonly WikiArticles _wikiArticles;
    private readonly IdentityRepository _identityRepository;

    [BindProperty]
    public WIkiArticleInput Input { get; set; } = new();
    public SelectListItem[] Users { get; set; } = Array.Empty<SelectListItem>();
    public bool IsDisabled { get; set; }

    public EditArticleModel(
        WikiArticles wikiArticles, 
        IdentityRepository identityRepository)
    {
        _wikiArticles = wikiArticles;
        _identityRepository = identityRepository;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var dto = await UpdateView(id, cancellationToken);
        if (dto is not null)
        {
            Input = dto;
        }
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        var currentDto = await UpdateView(id, cancellationToken);
        if (currentDto is null) return Page();

        if (Input.IsDescriptionEmpty)
        {
            ModelState.AddModelError(string.Empty, "Eine Beschreibung wird benötigt.");
            return Page();
        }

        var isSameIdentifier = currentDto.Identifier!.Equals(Input.Identifier!.Trim(), StringComparison.OrdinalIgnoreCase);

        if (!isSameIdentifier &&
            await _wikiArticles.HasIdentifier(Input.Identifier!, cancellationToken))
        {
            ModelState.AddModelError(string.Empty, "Die Kennung existiert bereits.");
            return Page();
        }

        WikiArticleDto dto = Input;
        dto.Id = id;

        if (!await _wikiArticles.Update(dto, cancellationToken))
        {
            ModelState.AddModelError(string.Empty, "Fehler beim Speichern des Artikels.");
            return Page();
        }

        return RedirectToPage("Index");
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _wikiArticles.Delete(id, cancellationToken);
        return new JsonResult(result);
    }

    private async Task<WikiArticleDto?> UpdateView(Guid id, CancellationToken cancellationToken)
    {
        var dto = await _wikiArticles.Find(id, cancellationToken);
        if (dto == null)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, "Artikel wurde nicht gefunden.");
            return null;
        }

        var users = await _identityRepository.GetAll(cancellationToken);

        var items = new List<SelectListItem> { new() };

        items.AddRange(users
            .Where(u => u.Roles!.Any(r => r == Roles.Member))
            .Select(u => new SelectListItem(u.Name, u.Id.ToString(), u.Id == dto.UserId)));

        Users = items.ToArray();

        return ModelState.IsValid ? dto : null;
    }
}
