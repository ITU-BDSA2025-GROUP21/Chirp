using Chirp.Core.DTO;
using Chirp.Core.Models;
using Chirp.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

[Authorize]
public class InformationPageView : PageModel
{
    private readonly IAuthorService _authorService;

    private readonly ICheepService _cheepService;
    public IEnumerable<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
    public int CurrentPage { get; set; } //Tracker til pagination
    public string Author { get; set; } = string.Empty;  //Tracker til auhthor navn

    public InformationPageView(ICheepService service, IAuthorService authorService)
    {
        _cheepService = service;
        _authorService = authorService;
    }

    public ActionResult OnGet() //Pagination via query string
    {

        if(!_authorService.SignIn(User))
        {
            return RedirectToPage("/");
        }

        return Page();
    }
}
