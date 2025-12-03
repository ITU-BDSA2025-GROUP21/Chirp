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
    public IEnumerable<AuthorDTO> Following { get; set; } = new List<AuthorDTO>();

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

            await _cheepService.DeleteAllCheepsAsync(CurrentAuthor.Id);
            await _authorService.RemoveAllFollowers(User);
            await _authorService.DeleteAuthorByIdAsync(CurrentAuthor.Id);
            await _authorService.SignOutAsync();
        }

        return RedirectToPage("/PublicView");
    }

    public async Task<ActionResult> OnGet()
    {

        if (!_authorService.SignIn(User))
        {
            return RedirectToPage("/PublicView");
        }

        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            Following = await _authorService.GetFollowing(User);
        }

        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            var userAuthor = _authorService.GetCurrentIdentityAuthor(User);

            if (userAuthor != null)
            {
                // Search for my followers and my own cheeps

                var following = await _authorService.GetFollowing(User);

            }
        }
        return Page();
    }

    public async Task<ActionResult> OnPostUnfollow(string followeeId)
    {

        if (User.Identity == null || followeeId == null || !User.Identity.IsAuthenticated)
        {
            // throw some error idk.
            return RedirectToPage();
        }

        if ((await _authorService.IsFollowing(User, followeeId)))
        {
            await _authorService.UnfollowAuthor(User, followeeId);
        }

        return RedirectToPage();
    }

    public int GetCurrentAuthorCheepCount()
    {
        if (!_authorService.SignIn(User))
        {
            return 0;
        }

        return _cheepService.GetCheepsFromAuthorId(_authorService.GetCurrentIdentityAuthor(User).Email).Count();
    }

    public AuthorDTO GetAuthorDTO()
    {
        if (!_authorService.SignIn(User))
        {
            return null;
        }

        return _authorService.GetCurrentIdentityAuthor(User);
    }

    public async Task<IEnumerable<AuthorDTO>> GetFollowers()
    {
        if (!_authorService.SignIn(User))
        {
            return new List<AuthorDTO>();
        }

        return await _authorService.GetFollowers(User);
    }
}
