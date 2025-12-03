using Chirp.Core.DTO;
using Chirp.Core.Models;
using Chirp.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

[Authorize]
public class UserTimelineView : PageModel
{
    private readonly UserManager<Author> _userManager;

    private readonly ICheepService _cheepService;
    private readonly IAuthorService _authorService;
    public IEnumerable<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
    public IEnumerable<AuthorDTO> Following { get; set; } = new List<AuthorDTO>();
    public int CurrentPage { get; set; } //Tracker til pagination
    public string Author { get; set; } = string.Empty;  //Tracker til auhthor navn

    public UserTimelineView(ICheepService cheepService, IAuthorService authorService, UserManager<Author> userManager)
    {
        _cheepService = cheepService;
        _authorService = authorService;
        _userManager = userManager;
    }

    public ActionResult OnGet(string author, [FromQuery] int page = 1) //Pagination via query string
    {

        // Algorithm explained.
        //  - User Authed as Page User
        //      - Display Own Cheeps
        //      - Display Following Cheeps
        //  - Else
        //      - Display Page Cheeps

        if (page < 1) page = 1;

        Author = author;
        CurrentPage = page;

        if (User.Identity != null && User.Identity.IsAuthenticated && User.Identity.Name != null)
        {
            Following = _authorService.GetFollowing(
                _authorService.FindAuthorByEmail(User.Identity.Name).Name
                );
        }

        if (User.Identity != null && User.Identity.IsAuthenticated && User.Identity.Name != null)
        {
            var userAuthor = _authorService.FindAuthorByEmail(User.Identity.Name);

            if (userAuthor != null && author == User.Identity.Name)
            {
                // Search for my followers and my own cheeps

                var following = _authorService.GetFollowing(userAuthor.Name);

                Cheeps = _cheepService.GetCheepsFromMultipleAuthors(
                    following.Select(a => a.Name)
                    .Append(User.Identity.Name)
                    .ToList(), page);
            } 
            else
            {
                Cheeps = _cheepService.GetCheepsFromAuthor(author, page);
            }
        } 
        else
        {
            Cheeps = _cheepService.GetCheepsFromAuthor(author, page);
        }

        return Page();
    }

    public async Task<Author?> GetCurrentAuthorAsync()
    {
        return await _userManager.GetUserAsync(User);
    }

    public ActionResult OnPostToggleFollow(string followee)
    {
        // grab my current user.

        if (User.Identity == null || followee == null || !User.Identity.IsAuthenticated)
        {
            // throw some error idk.
            return RedirectToPage();
        }

        var currentUser = _authorService.FindAuthorByEmail(User.Identity.Name);

        if (!_authorService.IsFollowing(currentUser.Name, followee))
        {
            _authorService.FollowAuthor(currentUser.Name, followee);
        }
        else
        {
            _authorService.UnfollowAuthor(currentUser.Name, followee);
        }

        return RedirectToPage();
    }
}
