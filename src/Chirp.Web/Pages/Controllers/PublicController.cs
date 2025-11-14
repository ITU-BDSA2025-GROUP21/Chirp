using Chirp.Core.DTO;
using Chirp.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages.Controllers;

public class PublicController : PageModel
{
    [BindProperty]
    public string? Text { get; set; }
    
    private readonly ICheepService _service;
    public IEnumerable<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
    public int CurrentPage { get; set; } //Tracker til pagination

    public PublicController(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet([FromQuery] int page = 1) //Pagination via query string
    {
        if (page < 1) page = 1; //Sikrer at page ikke er mindre end 1

        CurrentPage = page;
        Cheeps = _service.GetCheeps(page);
        return Page();
    }
    
    public IActionResult OnPost([FromQuery] int page = 1)
    {
        if (page < 1) page = 1;

        // Only create cheep if something was typed
        if (!string.IsNullOrWhiteSpace(Text))
        {
            // Adjust to your service API – guessing something like this:
            _service.MakeCheep(new CheepDTO
            {
                Author = User.Identity.Name,
                Message = Text,
                CreatedDate =  DateTime.Now.ToString()
            });
        }

        CurrentPage = page;
        Cheeps = _service.GetCheeps(page);

        return Page();
    }
}