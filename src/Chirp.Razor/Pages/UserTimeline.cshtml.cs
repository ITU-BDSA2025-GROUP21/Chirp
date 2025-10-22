using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public IEnumerable<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
    public int CurrentPage { get; set; } //Tracker til pagination
    public string Author { get; set; } = string.Empty;  //Tracker til auhthor navn

    public UserTimelineModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet(string author, [FromQuery] int page = 1) //Pagination via query string
    {
        if (page < 1) page = 1;
        
        Author = author;
        CurrentPage = page;
        Cheeps = _service.GetCheepsFromAuthor(author, page);
        return Page();
    }
}
