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
    public IEnumerable<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
    public int CurrentPage { get; set; }

    private readonly ICheepService _service;

    public PublicView(ICheepService service)
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




}
