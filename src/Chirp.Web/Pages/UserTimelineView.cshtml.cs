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
    public int CurrentPage { get; set; }          // Tracker til pagination

    public string CurrentAuthorName { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty; // Tracker til author-navn

    public UserTimelineView(ICheepService cheepService, IAuthorService authorService)
    {
        _cheepService = cheepService;
        _authorService = authorService;
    }

    // /{authorEmail}?page=1
    public ActionResult OnGet(string authorEmail, [FromQuery] int page = 1)
    {
        if (page < 1) page = 1;

        Author = authorEmail;
        CurrentPage = page;

        Following = new List<AuthorDTO>();
        AuthorDTO? currentAuthor = null;

        // Find nuværende logged-in author
        if (User.Identity != null && User.Identity.IsAuthenticated && User.Identity.Name != null)
        {
            currentAuthor = _authorService.FindAuthorByEmail(User.Identity.Name);

            if (currentAuthor != null)
            {
                CurrentAuthorName = currentAuthor.Name;
                Following = _authorService.GetFollowing(currentAuthor.Name)
                            ?? new List<AuthorDTO>();
            }
        }

        // Hvis det er MIN egen timeline: vis mig + dem jeg følger
        if (currentAuthor != null && authorEmail == User.Identity?.Name)
        {
            var authors = Following
                .Select(a => a.Name)
                .Append(currentAuthor.Name)
                .ToList();

            Cheeps = _cheepService.GetCheepsFromMultipleAuthors(authors, page);
        }
        else
        {
            // Anden bruger: vis kun deres cheeps
            Cheeps = _cheepService.GetCheepsFromAuthorEmail(authorEmail, page);
        }

        return Page();
    }

    public ActionResult OnPostToggleFollow(string followee)
    {
        if (User.Identity == null || followee == null || !User.Identity.IsAuthenticated)
        {
            return RedirectToPage();
        }

        var currentUser = _authorService.FindAuthorByEmail(User.Identity.Name);

        if (currentUser == null)
        {
            return RedirectToPage();
        }

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
