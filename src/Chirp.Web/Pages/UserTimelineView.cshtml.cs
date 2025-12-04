using Chirp.Core.DTO;
using Chirp.Core.Models;
using Chirp.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;
public class UserTimelineView : PageModel
{
    private readonly ICheepService _cheepService;
    private readonly IAuthorService _authorService;
    private readonly IIdentityUserService _identityService;
    public IEnumerable<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
    public IEnumerable<AuthorDTO> Following { get; set; } = new List<AuthorDTO>();
    public int CurrentPage { get; set; } //Tracker til pagination
    public AuthorDTO Author { get; set; } = null;  //Tracker til author navn
    public UserTimelineView(ICheepService cheepService, IAuthorService authorService, IIdentityUserService identityService)
    {
        _cheepService = cheepService;
        _authorService = authorService;
        _identityService = identityService;
    }

    public async Task<ActionResult> OnGet(string authorId, [FromQuery] int page = 1) //Pagination via query string
    {

        Author = _authorService.FindAuthorById(authorId);

        if (Author == null)
        {
            return Page();
        }

        if (_identityService.IsSignedIn(User) && authorId == Author.Id)
        {
            Cheeps = _cheepService.GetCheepsFromMultipleAuthors(
                Following.Select(a => a.Id)
                .Append(authorId)
                .ToList(), page);
        }

        Following = _authorService.GetFollowing(authorId);

        if (!Cheeps.Any())
        {
            Cheeps = _cheepService.GetCheepsFromAuthorId(authorId, page);
        }

        return Page();
    }

    public async Task<ActionResult> OnPostToggleFollow(string followeeId)
    {
        // grab my current user.

        if (!_identityService.IsSignedIn(User))
        {
            return RedirectToPage();
        }

        var userAuthor = await _identityService.GetCurrentIdentityAuthor(User);

        if (!_authorService.IsFollowing(userAuthor.Id, followeeId))
        {
            _authorService.FollowAuthor(userAuthor.Id, followeeId);
        }
        else
        {
            _authorService.UnfollowAuthor(userAuthor.Id, followeeId);
        }

        return RedirectToPage();
    }

    public string GetUserName(string id)
    {
        return _authorService.FindAuthorById(id).Name;
    }

    //handle likes and dislikes
    public async Task<IActionResult> OnPostCheepLikeAsync(int cheepId)
    {
        _cheepService.Like(cheepId, (await _identityService.GetCurrentIdentityAuthor(User)).Id, true);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCheepDislikeAsync(int cheepId)
    {
        _cheepService.Like(cheepId, (await _identityService.GetCurrentIdentityAuthor(User)).Id, false);
        return RedirectToPage();
    }
}
