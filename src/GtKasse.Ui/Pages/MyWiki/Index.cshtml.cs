using GtKasse.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKasse.Ui.Pages.MyWiki;

[Node("Mein Wiki", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,member")]
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
        Items = await _wikiArticles.GetList(
            User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Chairperson),
            cancellationToken);
    }
}
