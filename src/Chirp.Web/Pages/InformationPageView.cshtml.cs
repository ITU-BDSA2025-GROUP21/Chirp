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
    private readonly IIdentityUserService _identityService;
    public AuthorDTO? Author { get; set; } = null;  //Tracker til author navn
    public int CurrentCheeps { get; set; } = 0;

    public IEnumerable<AuthorDTO?> Following { get; set; } = new List<AuthorDTO>();

    public InformationPageView(ICheepService service, IAuthorService authorService, IIdentityUserService identityService)
    {
        _cheepService = service;
        _authorService = authorService;
        _identityService = identityService;
    }

    public async Task<IActionResult> OnPostForgetMe()
    {
        if (_identityService.IsSignedIn(User))
        {
            AuthorDTO? CurrentAuthor = await _identityService.GetCurrentIdentityAuthor(User);

            if (CurrentAuthor == null)
                return Page();

            await _cheepService.DeleteAllCheepsAsync(CurrentAuthor.Id);
            _authorService.RemoveAllFollowers(CurrentAuthor.Id);
            await _authorService.DeleteAuthorByIdAsync(CurrentAuthor.Id);
            await _identityService.SignOutAsync();
        }

        return RedirectToPage("/PublicView");
    }

    public async Task<ActionResult> OnGet()
    {

        if (!_identityService.IsSignedIn(User))
        {
            return RedirectToPage("/PublicView");
        }

        Author = await _identityService.GetCurrentIdentityAuthor(User);

        if (Author != null)
        {
            Following = _authorService.GetFollowing(Author.Id);
            CurrentCheeps = _cheepService.GetCheepsFromAuthorId(Author.Id).Count();
        }
        return Page();
    }

    public async Task<ActionResult> OnPostUnfollow(string followeeId)
    {

        AuthorDTO? currentAuthor = await _identityService.GetCurrentIdentityAuthor(User);

        if (currentAuthor == null)
        {
            // throw some error idk.
            return RedirectToPage();
        }

        if (_authorService.IsFollowing(currentAuthor.Id, followeeId))
        {
            _authorService.UnfollowAuthor(currentAuthor.Id, followeeId);
        }

        return RedirectToPage();
    }

    public async Task<AuthorDTO?> GetAuthorDTO()
    {
        if (!_identityService.IsSignedIn(User))
        {
            return null;
        }

        AuthorDTO? author = await _identityService.GetCurrentIdentityAuthor(User);

        return author;
    }


}
