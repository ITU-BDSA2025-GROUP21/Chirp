using Chirp.Core.DTO;
using Chirp.Core.Models;
using Chirp.Core.Repositories;
using Chirp.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

[Authorize]
public class UserTimelineView : PageModel
{
    
    private readonly ICheepService _service;
    public IEnumerable<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
    public int CurrentPage { get; set; } //Tracker til pagination
    public string Author { get; set; } = string.Empty;  //Tracker til auhthor navn

    public UserTimelineView(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet(string author, [FromQuery] int page = 1) //Pagination via query string
    {
        if (page < 1) page = 1;
        
        Author = author;
        CurrentPage = page;
        Cheeps = _service.GetCheepsFromAuthorEmail(author, page);
        return Page();
    }
}
