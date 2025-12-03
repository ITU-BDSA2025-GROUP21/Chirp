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
    
    public string CurrentAuthorName { get; set; }
    public string Author { get; set; } = string.Empty;  //Tracker til auhthor navn

    public UserTimelineView(ICheepService cheepService, IAuthorService authorService, UserManager<Author> userManager)
    {
        _cheepService = cheepService;
        _authorService = authorService;
        _userManager = userManager;
    }

    public ActionResult OnGet(string author, [FromQuery] int page = 1)
    {
        if (page < 1) page = 1;

        Author = author;
        CurrentPage = page;

        Following = new List<AuthorDTO>();

        AuthorDTO? currentAuthor = null;

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

        if (currentAuthor != null && author == User.Identity.Name)
        {
            var following = _authorService.GetFollowing(currentAuthor.Name) 
                            ?? new List<AuthorDTO>();

            var authors = following
                .Select(a => a.Name)
                .Append(currentAuthor.Name)
                .ToList();

            Cheeps = _cheepService.GetCheepsFromMultipleAuthors(authors, page);
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
