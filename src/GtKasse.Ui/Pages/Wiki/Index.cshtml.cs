namespace GtKasse.Ui.Pages.Wiki;

using GtKasse.Core.Models;
using GtKasse.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Wiki", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,chairperson")]
public class IndexModel : PageModel
{
    private readonly WikiArticles _wikiArticles;

    public WikiArticleListDto[] Items { get; set; } = Array.Empty<WikiArticleListDto>();

    public IndexModel(WikiArticles wikiArticles)
    {
        _wikiArticles = wikiArticles;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Items = await _wikiArticles.GetList(true, cancellationToken);
    }
}
