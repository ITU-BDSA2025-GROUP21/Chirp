using AspNetCoreGeneratedDocument;
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

    public InformationPageView(ICheepService service, IAuthorService authorService)
    {
        _cheepService = service;
        _authorService = authorService;
    }

    public async Task<IActionResult> OnPostForgetMe()
    {
        if (_authorService.SignIn(User))
        {
            AuthorDTO CurrentAuthor = _authorService.GetCurrentIdentityAuthor(User);

            Console.WriteLine("I RAN");
            await _cheepService.DeleteAllCheepsAsync(CurrentAuthor.Id);
            await _authorService.DeleteAuthorByIdAsync(CurrentAuthor.Id);
            await _authorService.SignOutAsync();
        } else
        {
            Console.WriteLine("I AM NULL");
        }

            return RedirectToPage("/PublicView");
    } 

    public ActionResult OnGet() //Pagination via query string
    {

        if(!_authorService.SignIn(User))
        {
            return RedirectToPage("/PublicView");
        }
        return Page();
    }

    public int GetCurrentAuthorCheepCount()
    {
        if(!_authorService.SignIn(User))
        {
            return 0;
        }

        return _cheepService.GetCheepsFromAuthorEmail(_authorService.GetCurrentIdentityAuthor(User).Email).Count();
    }

    public AuthorDTO GetAuthorDTO()
    {
        if(!_authorService.SignIn(User))
        {
            return null;
        }

        return _authorService.GetCurrentIdentityAuthor(User);
    }
}
