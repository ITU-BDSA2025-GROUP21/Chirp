using Chirp.Core.DTO;
using Chirp.Core.Models;
using Chirp.Core.Repositories;
using Chirp.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicView : PageModel
{
    [BindProperty]
    public string? Text { get; set; }
    
    public IEnumerable<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
    public IEnumerable<AuthorDTO?> Following { get; set; } = new List<AuthorDTO>();
    public int CurrentPage { get; set; }
    public AuthorDTO? IdentityAuthor { get; set; }

    public AuthorDTO? Author { get; set; } = null;  //Tracker til author navn

    private readonly ICheepService _cheepService;
    private readonly IAuthorService _authorService;
    private readonly IIdentityUserService _identityService;

    public PublicView(ICheepService cheepService, IAuthorService authorService, IIdentityUserService identityService)
    {
        _cheepService = cheepService;
        _authorService = authorService;
        _identityService = identityService;
    }

    public async Task<ActionResult> OnGet([FromQuery] int page = 1) //Pagination via query string
    {
        if (page < 1) page = 1; //Sikrer at page ikke er mindre end 1

        CurrentPage = page;
        Cheeps = _cheepService.GetCheeps(page);

        if (_identityService.IsSignedIn(User))
        {
            IdentityAuthor = await _identityService.GetCurrentIdentityAuthor(User);

            if(IdentityAuthor == null)
            {
                return Page();
            }

            Following = _authorService.GetFollowing(IdentityAuthor.Id);
        }

        return Page();
    }
    
    public async Task<IActionResult> OnPost([FromQuery] int page = 1)
    {
        if (page < 1) page = 1;

        AuthorDTO? author = await _identityService.GetCurrentIdentityAuthor(User);

        if(author == null)
        {
            return Page();
        }

        IdentityAuthor = author;
        Following = _authorService.GetFollowing(author.Id);

        if (!string.IsNullOrWhiteSpace(Text))
        {
            _cheepService.AddCheep(Text, author.Id);
        }

        CurrentPage = page;
        Cheeps = _cheepService.GetCheeps(page);
        Text = string.Empty;
        ModelState.Clear();
        return Page();
    }

    public async Task<ActionResult> OnPostToggleFollow(string followeeId)
    {
        // grab my current user.
        if (!_identityService.IsSignedIn(User))
        {
            // throw some error idk.
            return RedirectToPage();
        }

        AuthorDTO? author = await _identityService.GetCurrentIdentityAuthor(User); 

        if(author == null)
        {
            return RedirectToPage();
        }

        if (!_authorService.IsFollowing(author.Id, followeeId))
        {
            _authorService.FollowAuthor(author.Id, followeeId);
        } 
        else
        {
            _authorService.UnfollowAuthor(author.Id, followeeId);
        }

        return RedirectToPage();
    }

    public async Task<string> GetUserName()
    {
        AuthorDTO? author = await _identityService.GetCurrentIdentityAuthor(User);

        if (author == null)
            return string.Empty;

        return author?.Name ?? "Anon";
    }

    //handle likes and dislikes
    public async Task<IActionResult> OnPostCheepLikeAsync(int cheepId, string userId)
    {
        if (!_identityService.IsSignedIn(User))
            return RedirectToPage();

        var currentAuthor = await _identityService.GetCurrentIdentityAuthor(User);
        if (currentAuthor == null)
            return RedirectToPage();

        Likes like = _cheepService.getLike(cheepId, currentAuthor.Id, true);

        _cheepService.Like(cheepId, currentAuthor.Id, true);

        string authorId = _cheepService.GetById(cheepId).AuthorId;

        if (like.likeStatus == 1)
        {
            _authorService.changeKarma(-10, authorId);
        }
        else
        {
            _authorService.changeKarma(10, authorId);
        }

        // Redirect back to the same author’s page
        return RedirectToPage("/PublicView", new { authorId = userId, page = CurrentPage });
    }

    public async Task<IActionResult> OnPostCheepDislikeAsync(int cheepId, string userId)
    {
        if (!_identityService.IsSignedIn(User))
            return RedirectToPage();

        var currentAuthor = await _identityService.GetCurrentIdentityAuthor(User);
        if (currentAuthor == null)
            return RedirectToPage();

        Likes like = _cheepService.getLike(cheepId, currentAuthor.Id, false);

        _cheepService.Like(cheepId, currentAuthor.Id, false);

        string authorId = _cheepService.GetById(cheepId).AuthorId;

        if (like.likeStatus == -1)
        {
            _authorService.changeKarma(10, authorId);
        }
        else
        {
            _authorService.changeKarma(-10, authorId);
        }

        // Redirect back to the same author’s page
        return RedirectToPage("/PublicView", new { authorId = userId, page = CurrentPage });
    }
}
