namespace GtKasse.Ui.Pages.MyWiki;

using GtKasse.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Artikel anzeigen", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,member")]
public class ShowArticleModel : PageModel
{
    private WikiArticles _wikiArticles;

    public WikiArticleDto? Item { get; private set; }

    public ShowArticleModel(WikiArticles wikiArticles)
    {
        _wikiArticles = wikiArticles;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        Item = await _wikiArticles.Find(id, cancellationToken);
    }
}
