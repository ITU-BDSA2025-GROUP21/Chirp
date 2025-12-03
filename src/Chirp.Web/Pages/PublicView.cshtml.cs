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
    public IEnumerable<AuthorDTO> Following { get; set; } = new List<AuthorDTO>();
    public int CurrentPage { get; set; }

    private readonly ICheepService _cheepService;
    private readonly IAuthorService _authorService;

    public PublicView(ICheepService cheepService, IAuthorService authorService)
    {
        _cheepService = cheepService;
        _authorService = authorService;
    }

    public async Task<ActionResult> OnGet([FromQuery] int page = 1) //Pagination via query string
    {
        if (page < 1) page = 1; //Sikrer at page ikke er mindre end 1

        CurrentPage = page;
        Cheeps = _cheepService.GetCheeps(page);
        
        if (User.Identity != null && User.Identity.IsAuthenticated && User.Identity.Name != null)
        {

            Following = await _authorService.GetFollowing(User);

        }

        return Page();
    }
    
    public IActionResult OnPost([FromQuery] int page = 1)
    {
        if (page < 1) page = 1;

        AuthorDTO? author = _authorService.GetCurrentIdentityAuthor(User);

        if(author == null)
        {
            return Page();
        }


        if (!string.IsNullOrWhiteSpace(Text))
        {
            _cheepService.AddCheep(Text, author.Id);
        }

        CurrentPage = page;
        Cheeps = _cheepService.GetCheeps(page);

        return Page();
    }

    public async Task<ActionResult> OnPostToggleFollow(string followeeId)
    {
        // grab my current user.
        if (User.Identity == null || followeeId == null || !User.Identity.IsAuthenticated)
        {

            Console.WriteLine($"followId null {followeeId == null}");
            Console.WriteLine($"User.Identity null {User.Identity == null}");

            if(User.Identity != null)
            {
                Console.WriteLine($"User.Identity.Authenticated {User.Identity.IsAuthenticated}");
            }
            // throw some error idk.
            return RedirectToPage();
        }



        if (!(await _authorService.IsFollowing(User, followeeId)))
        {
            await _authorService.FollowAuthor(User, followeeId);
        } 
        else
        {
            Console.WriteLine("RUNNED AGAIN BITCH");
            await _authorService.UnfollowAuthor(User, followeeId);
        }

        return RedirectToPage();
    }

    public string GetUserName()
    {
        return _authorService.GetCurrentIdentityAuthor(User).Name;
    }
}
