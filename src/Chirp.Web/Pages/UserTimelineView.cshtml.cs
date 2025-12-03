using Chirp.Core.DTO;
using Chirp.Core.Models;
using Chirp.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

[Authorize]
public class UserTimelineView : PageModel
{

    private readonly ICheepService _cheepService;
    private readonly IAuthorService _authorService;
    public IEnumerable<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
    public IEnumerable<AuthorDTO> Following { get; set; } = new List<AuthorDTO>();
    public int CurrentPage { get; set; } //Tracker til pagination
    public string Author { get; set; } = string.Empty;  //Tracker til auhthor navn
    public UserTimelineView(ICheepService cheepService, IAuthorService authorService)
    {
        _cheepService = cheepService;
        _authorService = authorService;
    }

    public async Task<ActionResult> OnGet(string authorId, [FromQuery] int page = 1) //Pagination via query string
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            Following = await _authorService.GetFollowing(User);
        }

        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            var userAuthor = _authorService.GetCurrentIdentityAuthor(User);

            if (userAuthor != null && authorId == userAuthor.Id)
            {
                // Search for my followers and my own cheeps

                var following = await _authorService.GetFollowing(User);

                Cheeps = _cheepService.GetCheepsFromMultipleAuthors(
                    following.Select(a => a.Id)
                    .Append(userAuthor.Id)
                    .ToList(), page);
            }
            else
            {
                Cheeps = _cheepService.GetCheepsFromAuthorId(authorId, page);
            }
        }
        else
        {
            Cheeps = _cheepService.GetCheepsFromAuthorId(authorId, page);
        }

        return Page();
    }

    public async Task<ActionResult> OnPostToggleFollow(string followeeId)
    {
        // grab my current user.

        if (User.Identity == null || followeeId == null || !User.Identity.IsAuthenticated)
        {
            // throw some error idk.
            return RedirectToPage();
        }

        if (!(await _authorService.IsFollowing(User, followeeId)))
        {
            await _authorService.FollowAuthor(User, followeeId);
        }
        else
        {
            await _authorService.UnfollowAuthor(User, followeeId);
        }

        return RedirectToPage();
    }
}