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
    public int AmountOfCheeps { get; set; } = 0;
    public string AuthorName { get; set; } = string.Empty; 
    public string AuthorEmail { get; set; } = string.Empty;
    

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
